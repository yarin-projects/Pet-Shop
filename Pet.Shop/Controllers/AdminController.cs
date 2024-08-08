using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Repositories;
using PetShop.Services.Controller;
using PetShop.Services.Images;
using PetShop.Services.Users;
using PetShop.Services.Validations;

namespace PetShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IPetShopRepository repository, IImageHandler imageHandler, IControllerHelper controllerHelper, IUserService userService, IValidationService validationService) : Controller
    {
        private readonly IPetShopRepository _repository = repository;
        private readonly IImageHandler _imageHandler = imageHandler;
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
        public IActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                var animal = _repository.GetAnimalById(id.Value);
                if (animal != null)
                {
                    AnimalCreateVM animalViewModel = GetModelData(animal);
                    return View(animalViewModel);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimalCreateVM model)
        {
            if (!model.AnimalId.HasValue)
            {
                ModelState.AddModelError("AnimalId", "Invalid animal ID.");
            }
            if (_validationService.IsAnimalValid(ModelState, model))
            {
                return await UpdateAnimal(model);
            }
            var animal = _repository.GetAnimalById(model.AnimalId!.Value);
            if (animal != null)
            {
                model = GetModelData(animal);
            }
            return View(model);
        }
        [HttpDelete]
        public IActionResult DeleteAnimal(int animalId)
        {
            var animal = _repository.GetAnimalById(animalId);
            if (animal == null)
            {
                return NotFound();
            }
            _repository.DeleteAnimal(animalId);
            return Ok();
        }
        [Route("admin/create/animal")]
        public IActionResult CreateAnimal()
        {
            var model = new AnimalCreateVM
            {
                Categories = _repository.GetAllCategories()
            };
            return View(model);
        }
        [Route("admin/create/animal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnimal(AnimalCreateVM model)
        {
            if (_validationService.IsAnimalValid(ModelState, model))
            {
                return await SaveAnimal(model);
            }
            model.Categories = _repository.GetAllCategories();
            return View(model);
        }

        [Route("admin/create/category")]
        public IActionResult CreateCategory()
        {
            return View(new CategoryCreateVM());
        }

        [HttpPost]
        [Route("admin/create/category")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(CategoryCreateVM model)
        {
            var categories = _repository.GetAllCategoryNames();
            if (categories.Contains(model.Name, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Name", "Category name already exists.");
            }

            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name
                };
                _repository.AddCategory(category);
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Categories = categories;
            return View(model);
        }
        public IActionResult DeleteGuestComments()
        {
            _repository.DeleteGuestComments();
            return Ok();
        }
        [Route("admin/edit/comments/{id}")]
        public IActionResult EditComments(int? id)
        {
            if (id.HasValue)
            {
                var animal = _repository.GetAnimalByIdWithDecryptedUsernames(id.Value);
                if (animal != null)
                {
                    return View(animal);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(CommentCreateVM commentViewModel)
        {
            if (_validationService.IsCommentValid(ModelState, commentViewModel))
                return SaveComment(commentViewModel);

            var animal = _repository.GetAnimalByIdWithDecryptedUsernames(commentViewModel.AnimalId);
            if (animal != null)
                return View("EditComments", animal);
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public IActionResult DeleteEmptyCategories()
        {
            _repository.DeleteEmptyCategories();
            return Ok();
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
            return RedirectToAction("EditComments", new { id = comment.AnimalId });
        }
        private AnimalCreateVM GetModelData(Animal animal)
        {
            IFormFile picture = _imageHandler.GetImageFromDir(animal.PictureName);
            var animalViewModel = new AnimalCreateVM
            {
                Name = animal.Name,
                Age = animal.Age,
                Picture = picture,
                Description = animal.Description,
                CategoryId = animal.CategoryId,
                Categories = _repository.GetAllCategories(),
                AnimalId = animal.AnimalId
            };
            return animalViewModel;
        }
        private async Task<IActionResult> UpdateAnimal(AnimalCreateVM model)
        {
            await _repository.UpdateAnimal(model);
            return StatusCode(201);
        }
        private async Task<IActionResult> SaveAnimal(AnimalCreateVM model)
        {
            var uniqueImageName = await _imageHandler.SaveImageToFileAsync(model.Picture);
            if (model.Age.HasValue && model.CategoryId.HasValue)
            {
                var animal = new Animal
                {
                    Name = model.Name,
                    Age = (byte)model.Age.Value,
                    PictureName = uniqueImageName,
                    Description = model.Description,
                    CategoryId = model.CategoryId.Value
                };
                _repository.AddAnimal(animal);
                return RedirectToAction("Index");
            }
            model.Categories = _repository.GetAllCategories();
            return View(model);
        }
    }
}
