using PetShop.Repositories;
using PetShop.Services.Cookies;
using PetShop.Services.Tokens;

namespace PetShop.Middleware
{
    public class TokenRefreshMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _configuration = configuration;

        public async Task Invoke(HttpContext context, IJwtTokenService tokenService, IPetShopRepository repository, ICookieService cookieService)
        {
            var accessToken = context.Request.Cookies[_configuration["Cookies:Access"]!];
            var refreshToken = context.Request.Cookies[_configuration["Cookies:Access"]!];

            if (string.IsNullOrWhiteSpace(accessToken) && !string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshTokenOnly = refreshToken.Split(':')[0];
                var encryptedUsername = refreshToken.Split(':')[1];
                var savedRefreshToken = repository.GetRefreshToken(encryptedUsername!);

                if (savedRefreshToken == refreshTokenOnly)
                {
                    var user = repository.GetUserByUsername(encryptedUsername);
                    var newAccessToken = tokenService.GenerateToken(user!);
                    var newRefreshToken = tokenService.GenerateRefreshToken();
                    _ = double.TryParse(_configuration["Jwt:AccessTokenExpire"]!, out double expireMinutes);
                    repository.SaveRefreshToken(encryptedUsername!, newRefreshToken);
                    newRefreshToken += $":{encryptedUsername}";

                    cookieService.AppendCookie(context.Response, _configuration["Cookies:Access"]!, newAccessToken, DateTime.UtcNow.AddMinutes(expireMinutes));
                    cookieService.AppendCookie(context.Response, _configuration["Cookies:Refresh"]!, newRefreshToken);

                    context.Request.Headers.Authorization = $"Bearer {newAccessToken}";
                }
            }
            await _next(context);
        }
    }
}
