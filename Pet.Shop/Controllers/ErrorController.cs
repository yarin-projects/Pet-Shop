using Microsoft.AspNetCore.Mvc;

namespace PetShop.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("ErrorView");
        }
    }
}
