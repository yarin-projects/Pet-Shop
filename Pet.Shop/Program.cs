using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.Data;
using PetShop.Middleware;
using PetShop.Repositories;
using PetShop.Services.Controller;
using PetShop.Services.Cookies;
using PetShop.Services.Encryption.AesEncryption;
using PetShop.Services.Encryption.Argon2Hashing;
using PetShop.Services.Images;
using PetShop.Services.Tokens;
using PetShop.Services.Users;
using PetShop.Services.Validations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PetShopContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseLazyLoadingProxies().UseSqlite(connectionString);
});
builder.Services.AddScoped<IPetShopRepository, PetShopRepository>();
builder.Services.AddScoped<IControllerHelper, ControllerHelper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IImageHandler, ImageHanlder>();
builder.Services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();
builder.Services.AddSingleton<IArgon2PasswordHasher, Argon2PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IValidationService,  ValidationService>();
builder.Services.AddSingleton<ICookieService,  CookieService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
var accessTokenName = builder.Configuration["Cookies:Access"];
var refreshTokenName = builder.Configuration["Cookies:Refresh"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (!string.IsNullOrWhiteSpace(context.Request.Cookies[refreshTokenName!]))
            {
                context.Token = context.Request.Cookies[accessTokenName!];
            }
            else
            {
                context.HttpContext.Response.Cookies.Delete(accessTokenName!);
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/account/accessdenied");
                context.HandleResponse();
            }
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.HttpContext.Response.Redirect("/account/accessdenied");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PetShopContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

app.UseStaticFiles();

app.UserTokenRefresh();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("Default", "{controller=home}/{action=index}/{id?}");

app.Run();