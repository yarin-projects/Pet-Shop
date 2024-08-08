using PetShop.Models;
using System.Security.Claims;

namespace PetShop.Services.Tokens
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}
