using Microsoft.AspNetCore.Mvc;
using PetShop.Repositories;

namespace PetShop.Controllers
{
    public class HomeController(IPetShopRepository repository) : Controller
    {
        private readonly IPetShopRepository _repository = repository;

        public IActionResult Index()
        {
            int animalNumber = 2;
            var animals = _repository.GetMostPopularAnimals(animalNumber);
            return View(animals);
        }
    }
}
