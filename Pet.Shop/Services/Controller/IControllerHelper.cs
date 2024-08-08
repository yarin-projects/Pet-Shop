using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Protocol.Core.Types;
using PetShop.Models;
using PetShop.Models.ViewModels;

namespace PetShop.Services.Controller
{
    public interface IControllerHelper
    {
        IEnumerable<Animal> SortAnimalsByParams(int? categoryId, string sortOrder, string isDescending);
    }
}
