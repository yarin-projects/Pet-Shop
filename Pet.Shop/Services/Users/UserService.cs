using PetShop.Models;
using PetShop.Repositories;

namespace PetShop.Services.Users
{
    public class UserService(IPetShopRepository repository, IConfiguration configuration) : IUserService
    {
        private readonly IPetShopRepository _repository = repository;
        private readonly IConfiguration _configuration = configuration;

        public User GetCurrentUser(HttpRequest request)
        {
            var refreshToken = request.Cookies[_configuration["Cookies:Refresh"]!];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return _repository.GetGuestUser();
            }

            var fullRefreshToken = refreshToken.Split(':');
            var refreshTokenOnly = fullRefreshToken[0];
            var encryptedUsername = fullRefreshToken[1];
            var savedRefreshToken = _repository.GetRefreshToken(encryptedUsername);

            if (savedRefreshToken == refreshTokenOnly)
            {
                return _repository.GetUserByUsername(encryptedUsername);
            }

            throw new UnauthorizedAccessException("Invalid refresh token.");
        }
    }

}
