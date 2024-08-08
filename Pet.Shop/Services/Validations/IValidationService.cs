using Microsoft.AspNetCore.Mvc.ModelBinding;
using PetShop.Models;
using PetShop.Models.ViewModels;

namespace PetShop.Services.Validations
{
    public interface IValidationService
    {
        bool ValidatePicture(IFormFile picture);
        bool IsCommentValid(ModelStateDictionary modelState, CommentCreateVM commentViewModel);
        bool IsAnimalValid(ModelStateDictionary modelState, AnimalCreateVM model);
        string ValidateUserUpdate(ModelStateDictionary modelState, UserUpdateVM userUpdateModel, string decryptedUsername, User user);
    }
}
