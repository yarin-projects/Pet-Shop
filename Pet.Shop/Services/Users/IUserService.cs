using PetShop.Models;

namespace PetShop.Services.Users
{
    public interface IUserService
    {
        User GetCurrentUser(HttpRequest request);
    }
}
