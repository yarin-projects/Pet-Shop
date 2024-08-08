using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Repositories;
using PetShop.Services.Cookies;
using PetShop.Services.Encryption.AesEncryption;
using PetShop.Services.Tokens;
using PetShop.Services.Users;
using PetShop.Services.Validations;

namespace PetShop.Controllers
{
    public class AccountController(IPetShopRepository repository, IConfiguration configuration, IJwtTokenService tokenService, IAesEncryptionHelper encryptionHelper, IUserService userService, ICookieService cookieService, IValidationService validationService) : Controller
    {
        private readonly IPetShopRepository _repository = repository;
        private readonly IConfiguration _configuration = configuration;
        private readonly IJwtTokenService _tokenService = tokenService;
        private readonly IAesEncryptionHelper _encryptionHelper = encryptionHelper;
        private readonly IUserService _userService = userService;
        private readonly ICookieService _cookieService = cookieService;
        private readonly IValidationService _validationService = validationService;

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM userModel)
        {
            if (ModelState.IsValid)
            {
                var user = _repository.AuthenticateUser(userModel, out bool isPasswordWrong);
                if (user != null && user.Role.Name != "Guest")
                {
                    if (LoginUser(user))
                        return RedirectToAction("Manage", "Account");
                }
                else if (isPasswordWrong)
                {
                    ModelState.AddModelError("Password", "Wrong password.");
                }
                else
                {
                    ModelState.AddModelError("Username", "User doesn't exist.");
                }
            }
            return View(userModel);
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Manage()
        {
            var user = _userService.GetCurrentUser(Request);
            if (user.Comments.Count > 1)
                user.Comments = user.Comments.OrderBy(x => x.AnimalId).ToArray();
            if (user.Role.Name == "Admin")
            {
                var adminProfileModel = new AdminProfileVM
                {
                    User = user,
                    CommentsOrderedByUser = _repository.GetAllCommentsOrderedByUsers(),
                    CommentsOrderedByAnimal = _repository.GetAllCommentsOrderedByAnimals(),
                    Users = _repository.GetAllUsers(user.UserId)
                };
                return View("ManageAdmin", adminProfileModel);
            }
            return View(user);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [Route("/account/manage/update")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult UpdateAccount()
        {
            var user = _userService.GetCurrentUser(Request);
            var updateUserModel = new UserUpdateVM
            {
                UserId = user.UserId,
                Username = _encryptionHelper.Decrypt(user.Username!),
                OldPassword = user.Password
            };
            return View(updateUserModel);
        }
        [Route("/account/manage/update")]
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAccount(UserUpdateVM userUpdateModel)
        {
            var user = _userService.GetCurrentUser(Request);
            var decryptedUsername = _encryptionHelper.Decrypt(user.Username!);
            string validationResult = _validationService.ValidateUserUpdate(ModelState, userUpdateModel, decryptedUsername, user);
            User updatedUser;
            switch(validationResult)
            {
                case "NoChange":
                    return RedirectToAction("manage");
                case "BadRequest":
                    return BadRequest();
                case "UpdateAll":
                    updatedUser = _repository.UpdateAllUserDetails(userUpdateModel);
                    if (LoginUser(updatedUser))
                        return RedirectToAction("manage");
                    break;
                case "UpdatePassword":
                    updatedUser =  _repository.UpdateUserPassword(userUpdateModel);
                    if (LoginUser(updatedUser))
                        return RedirectToAction("manage");
                    break;
                case "UpdateUsername":
                    updatedUser = _repository.UpdateUserUsername(userUpdateModel);
                    if (LoginUser(updatedUser))
                        return RedirectToAction("manage");
                    break;
                default:
                    break;
            }
            return View(userUpdateModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM userModel)
        {
            if (ModelState.IsValid)
            {
                if (userModel.Password != userModel.PasswordConfirm)
                {
                    ModelState.AddModelError("Password", "Passwords must match.");
                }
                else if (!_repository.IsUserTaken(userModel.Username))
                {
                    var user = _repository.RegisterUser(userModel);
                    if (LoginUser(user))
                        return RedirectToAction("Manage", "Account");
                }
                else
                {
                    ModelState.AddModelError("Username", "User already exists");
                }
            }
            return View(userModel);
        }
        [Authorize(Roles = "Admin, User")]
        public IActionResult Logout()
        {
            _cookieService.ClearAccessCookies(Response);
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        public IActionResult DeleteAccount(int userId)
        {
            var user = _userService.GetCurrentUser(Request);
            if (user.Role.Name != "Admin" && userId! != user.UserId)
            {
                return Unauthorized();
            }
            if (user.UserId == userId)
            {
                if (user.Role.Name != "Admin")
                {
                    _repository.DeleteUserById(userId);
                }
            }
            return Ok();
        }
        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        public IActionResult DeleteUserComments(int userId)
        {
            var user = _userService.GetCurrentUser(Request);
            if (user.Role.Name != "Admin" && userId! != user.UserId)
            {
                return Unauthorized();
            }
            _repository.DeleteUserCommentsById(userId);
            return Ok();
        }
        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        public async Task<IActionResult> EditComment([FromBody] CommentCreateVM model)
        {
            var user = _userService.GetCurrentUser(Request);
            if (user.Role.Name != "Admin" && user.Comments.FirstOrDefault(x => x.CommentId == model.CommentId) == null)
            {
                ModelState.AddModelError("CommentId", "Can't edit a comment that isn't yours");
            }
            if (!model.CommentId.HasValue)
            {
                ModelState.AddModelError("CommentId", "Invalid comment ID.");
            }
            if (_validationService.IsCommentValid(ModelState, model))
            {
                return await UpdateComment(model);
            }
             
            var animal = _repository.GetAnimalByIdWithDecryptedUsernames(model.AnimalId);
            if (animal != null)
                return View("EditComments", animal);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var user = _userService.GetCurrentUser(Request);
            if (user.Role.Name != "Admin" && user.Comments.FirstOrDefault(x => x.CommentId == commentId) == null)
            {
                ModelState.AddModelError("CommentId", "Can't delete a comment that isn't yours");
            }
            else
            {
                var comment = await _repository.GetCommentById(commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                await _repository.DeleteCommentById(commentId);
            }
            return Ok();
        }
        private bool LoginUser(User user)
        {
            _cookieService.ClearAccessCookies(Response);
            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _ = double.TryParse(_configuration["Jwt:AccessTokenExpire"]!, out double expireMinutes);
            _repository.SaveRefreshToken(user.Username!, refreshToken);
            refreshToken += $":{user.Username}";

            _cookieService.AppendCookie(Response, _configuration["Cookies:Access"]!, accessToken, DateTime.UtcNow.AddMinutes(expireMinutes));
            _cookieService.AppendCookie(Response, _configuration["Cookies:Refresh"]!, refreshToken);

            return true;
        }
        private async Task<IActionResult> UpdateComment(CommentCreateVM model)
        {
            var comment = await _repository.GetCommentById(model.CommentId!.Value);
            if (comment == null)
            {
                return NotFound();
            }
            comment.Content = model.Content!;
            await _repository.UpdateComment(comment);
            return Ok();
        }
        
    }
}
