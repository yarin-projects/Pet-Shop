namespace PetShop.Services.Cookies
{
    public class CookieService(IConfiguration config) : ICookieService
    {
        private readonly IConfiguration _configuration = config;

        public void AppendCookie(HttpResponse response, string name, string value, DateTime? expires = null)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };
            response.Cookies.Append(name, value, cookieOptions);
        }
        public void ClearAccessCookies(HttpResponse response)
        {
            response.Cookies.Delete(_configuration["Cookies:Access"]!);
            response.Cookies.Delete(_configuration["Cookies:Refresh"]!);
        }
    }
}
