using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Build.ObjectModelRemoting;
using NuGet.Protocol.Core.Types;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Services.Encryption.Argon2Hashing;

namespace PetShop.Services.Validations
{
    public class ValidationService(IArgon2PasswordHasher passwordHasher) : IValidationService
    {
        private readonly IArgon2PasswordHasher _passwordHasher = passwordHasher;

        public bool ValidatePicture(IFormFile picture)
        {
            return picture == null || picture.Length < 1 || !picture.ContentType.StartsWith("image/");
        }
        public bool IsCommentValid(ModelStateDictionary modelState, CommentCreateVM commentViewModel)
        {
            if (commentViewModel.Content == null)
                return false;
            if (modelState.IsValid)
                return true;
            commentViewModel.Content = commentViewModel.Content.Trim();
            var normalizedDescriptionLength = NormalizeStringLength(commentViewModel.Content);
            var isContentValid = modelState.ErrorCount == 1 &&
                                 modelState["Content"]!.ValidationState == ModelValidationState.Invalid &&
                                 normalizedDescriptionLength < 101;
            return isContentValid;
        }
        public bool IsAnimalValid(ModelStateDictionary modelState, AnimalCreateVM model)
        {
            int normalizedDescriptionLength = NormalizeStringLength(model.Description);
            if (modelState.IsValid)
            {
                return true;
            }
            if (modelState.ErrorCount == 1)
            {
                if (modelState["Picture"]!.ValidationState == ModelValidationState.Invalid)
                {
                    return true;
                }
                else if (ValidatePicture(model.Picture))
                {
                    modelState.AddModelError("Picture", "Invalid picture.");
                }
                if (modelState["Description"]!.ValidationState == ModelValidationState.Invalid &&
                    normalizedDescriptionLength < 36)
                {
                    return true;
                }
            }
            else if (modelState.ErrorCount == 2 && 
                modelState["Description"]!.ValidationState == ModelValidationState.Invalid && 
                modelState["Picture"]!.ValidationState == ModelValidationState.Invalid && 
                normalizedDescriptionLength < 36)
            {
                return true;
            }
            return modelState.IsValid;
        }
        public string ValidateUserUpdate(ModelStateDictionary modelState, UserUpdateVM userUpdateModel, string decryptedUsername, User user)
        {
            string isUsernameValid = "";
            if (IsNoChangeRequested(userUpdateModel, decryptedUsername))
            {
                return "NoChange";
            }
            else if (user.UserId != userUpdateModel.UserId)
            {
                return "BadRequest";
            }
            else if (user.Role.Name != "Admin")
            {
                isUsernameValid = HandleUsernameUpdate(modelState, userUpdateModel, decryptedUsername);
            }
            if (isUsernameValid == "UpdateUsername")
                return isUsernameValid;
            if (modelState.IsValid && isUsernameValid != "Invalid")
            {
                return HandlePasswordUpdate(modelState, userUpdateModel, user, decryptedUsername);
            }
            return "Invalid";
        }
        private static int NormalizeStringLength(string str)
        {
            return str.Replace("\r\n", "\n")
                      .Replace("\r", "\n")
                      .Length;
        }
        private static bool IsNoChangeRequested(UserUpdateVM userUpdateModel, string decryptedUsername)
        {
            return decryptedUsername == userUpdateModel.Username &&
                   string.IsNullOrWhiteSpace(userUpdateModel.NewPassword) &&
                   string.IsNullOrWhiteSpace(userUpdateModel.ConfirmNewPassword) &&
                   string.IsNullOrWhiteSpace(userUpdateModel.OldPassword);
        }

        private string HandlePasswordUpdate(ModelStateDictionary modelState, UserUpdateVM userUpdateModel, User user, string decryptedUsername)
        {
            if (_passwordHasher.VerifyPassword(user.Password!, userUpdateModel.OldPassword!))
            {
                if (userUpdateModel.OldPassword == userUpdateModel.NewPassword)
                {
                    modelState.AddModelError("NewPassword", "Enter a new password if you would like to update it.");
                }
                else if (userUpdateModel.NewPassword == userUpdateModel.ConfirmNewPassword)
                {
                    if (decryptedUsername != userUpdateModel.Username && user.Role.Name != "Admin")
                    {
                        return "UpdateAll";
                    }
                    else
                    {
                        return "UpdatePassword";
                    }
                }
                else
                {
                    modelState.AddModelError("NewPassword", "New passwords must match.");
                }
            }
            else
            {
                modelState.AddModelError("OldPassword", "Old password is incorrect.");
            }
            return "Invalid";
        }
        private static string HandleUsernameUpdate(ModelStateDictionary modelState, UserUpdateVM userUpdateModel, string decryptedUsername)
        {
            if (modelState["Username"]!.ValidationState == ModelValidationState.Valid &&
                userUpdateModel.Username != decryptedUsername)
            {
                if (string.IsNullOrWhiteSpace(userUpdateModel.NewPassword) &&
                    string.IsNullOrWhiteSpace(userUpdateModel.ConfirmNewPassword) &&
                    string.IsNullOrWhiteSpace(userUpdateModel.OldPassword))
                {
                    return "UpdateUsername";
                }
                return "CheckPassword";
            }
            else if(userUpdateModel.Username == decryptedUsername)
            {
                return "CheckPassword";
            }
            return "Invalid";
        }
    }
}
