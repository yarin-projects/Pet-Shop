using Microsoft.AspNetCore.Mvc;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Repositories;
using PetShop.Services.Controller;
using PetShop.Services.Users;
using PetShop.Services.Validations;

namespace PetShop.Controllers
{
    public class CatalogController(IPetShopRepository repository, IControllerHelper controllerHelper, IUserService userService, IValidationService validationService) : Controller
    {
        private readonly IPetShopRepository _repository = repository;
        private readonly IControllerHelper _controllerHelper = controllerHelper;
        private readonly IUserService _userService = userService;
        private readonly IValidationService _validationService = validationService;

        public IActionResult Index(int? categoryId, string sortOrder, string isDescending = "false")
        {
            var animals = _controllerHelper.SortAnimalsByParams(categoryId, sortOrder, isDescending);
            var model = new AnimalCatalogVM
            {
                Animals = animals,
                Categories = _repository.GetAllCategories(),
                SelectedCategoryId = categoryId,
                SelectedSortOrder = sortOrder,
                IsDescending = isDescending
            };
            return View(model);
        }
        public IActionResult Animal(int? id)
        {
            if (id.HasValue)
            {
                var animal = _repository.GetAnimalByIdWithDecryptedUsernames(id.Value);
                if (animal != null)
                    return View(animal);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(CommentCreateVM commentViewModel)
        {
            if (_validationService.IsCommentValid(ModelState, commentViewModel))
                return SaveComment(commentViewModel);

            var animal = _repository.GetAnimalByIdWithDecryptedUsernames(commentViewModel.AnimalId);
            if (animal != null)
            {
                return View("Animal", animal);
            }
            return RedirectToAction("Index");
        }
        private RedirectToActionResult SaveComment(CommentCreateVM commentViewModel)
        {
            var user = _userService.GetCurrentUser(Request);
            var comment = new Comment
            {
                AnimalId = commentViewModel.AnimalId,
                Content = commentViewModel.Content!,
                User = user,
                UserId = user.UserId
            };
            _repository.AddComment(comment);
            return RedirectToAction("Animal", new { id = comment.AnimalId });
        }
        
    }
}
