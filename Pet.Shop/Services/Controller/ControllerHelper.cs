using Microsoft.AspNetCore.Mvc.ModelBinding;
using PetShop.Models;
using PetShop.Models.ViewModels;
using PetShop.Repositories;

namespace PetShop.Services.Controller
{
    public class ControllerHelper(IPetShopRepository repository) : IControllerHelper
    {
        private readonly IPetShopRepository _repository = repository;

        public IEnumerable<Animal> SortAnimalsByParams(int? categoryId, string sortOrder, string isDescending)
        {
            var animals = _repository.GetAllAnimals();
            var categories = _repository.GetAllCategories();
            if (categoryId.HasValue)
            {
                if (categories.FirstOrDefault(x => x.CategoryId == categoryId.Value) != null)
                    animals = animals.Where(a => a.CategoryId == categoryId.Value);
            }
            animals = sortOrder switch
            {
                "name" => isDescending == "true" ?
                            animals.OrderByDescending(a => a.Name) :
                            animals.OrderBy(a => a.Name),
                "comments" => isDescending == "true" ?
                            animals.OrderBy(a => a.Comments.Count) :
                            animals.OrderByDescending(a => a.Comments.Count),
                "category" => isDescending == "true" ?
                            animals.OrderByDescending(a => a.Category.Name) :
                            animals.OrderBy(a => a.Category.Name),
                _ => animals.OrderBy(a => a.Category.Name)
            };
            return animals;
        }
    }
}
