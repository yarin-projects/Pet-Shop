namespace PetShop.Middleware
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UserTokenRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRefreshMiddleware>();
        }
    }
}
