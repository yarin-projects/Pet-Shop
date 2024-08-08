using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Services.Encryption.AesEncryption;
using PetShop.Services.Encryption.Argon2Hashing;
using PetShop.Services.Images;

namespace PetShop.Repositories
{
    public class PetShopRepository(PetShopContext context, IImageHandler imageHandler,
                             IArgon2PasswordHasher passwordHasher, IAesEncryptionHelper encryptionHelper) : IPetShopRepository
    {
        private readonly PetShopContext _context = context;
        private readonly IImageHandler _imageHandler = imageHandler;
        private readonly IAesEncryptionHelper _encryptionHelper = encryptionHelper;
        private readonly IArgon2PasswordHasher _passwordHasher = passwordHasher;

        public IEnumerable<Animal> GetMostPopularAnimals(int animalNumber)
        {
            return _context.Animals
                .AsNoTracking()
                .OrderByDescending(x => x.Comments.Count)
                .Take(animalNumber);
        }
        public IEnumerable<Animal> GetAllAnimals()
        {
            return _context.Animals.AsNoTracking();
        }
        public Animal GetAnimalById(int animalId)
        {
            return _context.Animals.AsNoTracking().FirstOrDefault(x => x.AnimalId == animalId)!;
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.AsNoTracking();
        }
        public IEnumerable<string> GetAllCategoryNames()
        {
            return _context.Categories.AsNoTracking().Select(x => x.Name);
        }
        public void DeleteAnimal(int animalId)
        {
            Animal? animal = _context.Animals.FirstOrDefault(x => x.AnimalId == animalId);
            if (animal == null)
                return;
            _context.Animals.Remove(animal);
            _context.SaveChanges();
        }
        public void AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public void AddAnimal(Animal animal)
        {
            _context.Animals.Add(animal);
            _context.SaveChanges();
        }
        public async Task UpdateAnimal(AnimalCreateVM model)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(x => x.AnimalId == model.AnimalId!.Value);
            if (animal != null && model.Age.HasValue && model.CategoryId.HasValue)
            {
                animal.Name = model.Name;
                animal.Age = (byte)model.Age.Value;
                animal.Description = model.Description;
                animal.CategoryId = model.CategoryId.Value;
                if (model.Picture != null && model.Picture.FileName != animal.PictureName)
                {
                    animal.PictureName = await _imageHandler.SaveImageToFileAsync(model.Picture);
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Comment> GetCommentById(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            return comment!;
        }

        public async Task UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentById(int commentId)
        {
            var comment = await GetCommentById(commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
        public void DeleteEmptyCategories()
        {
            IEnumerable<Category> emptyCategories = _context.Categories
                                                    .Where(x => x.Animals.Count == 0)
                                                    .AsEnumerable();
            _context.Categories.RemoveRange(emptyCategories);
            _context.SaveChanges();
        }
        public User AuthenticateUser(LoginVM userModel, out bool isPasswordWrong)
        {
            var user = _context.Users
                .AsEnumerable()
                .FirstOrDefault(x => _encryptionHelper.Decrypt(x.Username!) == userModel.Username);
            if (user != null)
            {
                if (_passwordHasher.VerifyPassword(user.Password!, userModel.Password))
                {
                    isPasswordWrong = false;
                    return user;
                }
                else
                {
                    isPasswordWrong = true;
                    return null!;
                }
            }
            isPasswordWrong = false;
            return null!;
        }
        public bool IsUserTaken(string username)
        {
            var user = _context.Users
                .AsEnumerable()
                .FirstOrDefault(x => _encryptionHelper.Decrypt(x.Username!).Equals(username, StringComparison.OrdinalIgnoreCase));
            return user != null;
        }
        public User RegisterUser(RegisterVM userModel)
        {
            var encryptedUsername = _encryptionHelper.Encrypt(userModel.Username!);
            var hashedPassword = _passwordHasher.HashPassword(userModel.Password);
            var role = _context.Roles.FirstOrDefault(x => x.Name == "User");
            var user = new User { Username = encryptedUsername, Password = hashedPassword, Role = role! };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public void SaveRefreshToken(string username, string refreshToken)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                _context.SaveChanges();
            }
        }
        public string? GetRefreshToken(string username)
        {
            var user = GetUserByUsername(username);
            return user.RefreshToken;
        }
        public User GetUserByUsername(string? username)
        {
            var decryptedUsername = _encryptionHelper.Decrypt(username!);
            return _context.Users
                .AsEnumerable()
                .FirstOrDefault(x => _encryptionHelper.Decrypt(x.Username!) == decryptedUsername)!;
        }
        public void DeleteUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public User UpdateAllUserDetails(UserUpdateVM userUpdateModel)
        {
            var encryptedUsername = _encryptionHelper.Encrypt(userUpdateModel.Username!);
            var user = GetUserById(userUpdateModel.UserId);
            if (user != null)
            {
                user.Username = encryptedUsername;
                user.Password = _passwordHasher.HashPassword(userUpdateModel.NewPassword!);
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return user!;
        }
        public User UpdateUserUsername(UserUpdateVM userUpdateModel)
        {
            var encryptedUsername = _encryptionHelper.Encrypt(userUpdateModel.Username!);
            var user = GetUserById(userUpdateModel.UserId);
            if (user != null)
            {
                user.Username = encryptedUsername;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return user!;
        }
        public User UpdateUserPassword(UserUpdateVM userUpdateModel)
        {
            var user = GetUserById(userUpdateModel.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(userUpdateModel.NewPassword!);
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return user!;
        }
        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(x => x.UserId == userId)!;
        }

        public ICollection<Comment> GetAllCommentsOrderedByUsers()
        {
            var comments = _context.Comments
                                .AsNoTracking()
                                .OrderBy(x => x.UserId)
                                .ToArray();
            DecryptCommentUsernames(comments);
            return comments;
        }

        public ICollection<Comment> GetAllCommentsOrderedByAnimals()
        {
            var comments = _context.Comments
                                .AsNoTracking()
                                .OrderBy(x => x.AnimalId)
                                .ToArray();
            DecryptCommentUsernames(comments);
            return comments;
        }

        public void DeleteGuestComments()
        {
            var comments = _context.Comments
                                .AsEnumerable()
                                .Where(x => x.User.Role.Name == "Guest");
            _context.Comments.RemoveRange(comments);
            _context.SaveChanges();
        }

        public User GetGuestUser()
        {
            return _context.Users.FirstOrDefault(x => x.Role.Name == "Guest")!;
        }

        public Animal GetAnimalByIdWithDecryptedUsernames(int animalId)
        {
            var animal = _context.Animals
                                .AsNoTracking()
                                .FirstOrDefault(x => x.AnimalId == animalId);
            DecryptCommentUsernames(animal!.Comments);
            return animal;
        }
        public void DeleteUserCommentsById(int userId)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                var comments = user.Comments;
                _context.Comments.RemoveRange(comments);
                _context.SaveChanges();
            }
        }
        public ICollection<User> GetAllUsers(int userId)
        {
            var users = _context.Users
                            .AsNoTracking()
                            .Where(x => x.UserId != userId &&
                                        x.Role.Name == "User")
                            .ToArray();
            foreach (var user in users)
            {
                user.Username = _encryptionHelper.Decrypt(user.Username!);
            }
            return users;
        }
        private void DecryptCommentUsernames(ICollection<Comment> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.User.Role.Name == "Guest")
                {
                    comment.User.Username = "Guest";
                }
                else
                {
                    comment.User.Username = _encryptionHelper.Decrypt(comment.User.Username!);
                }
            }
        }
    }
}
