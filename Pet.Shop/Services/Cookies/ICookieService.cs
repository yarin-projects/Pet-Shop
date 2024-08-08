namespace PetShop.Services.Cookies
{
    public interface ICookieService
    {
        void AppendCookie(HttpResponse response, string name, string value, DateTime? expires = null);
        void ClearAccessCookies(HttpResponse response);
    }
}
