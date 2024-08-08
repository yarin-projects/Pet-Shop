using Microsoft.IdentityModel.Tokens;
using PetShop.Models;
using PetShop.Services.Encryption.AesEncryption;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetShop.Services.Tokens
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IAesEncryptionHelper _encryptionHelper;

        public JwtTokenService(IConfiguration configuration, IAesEncryptionHelper encryptionHelper)
        {
            _configuration = configuration;
            _encryptionHelper = encryptionHelper;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            double.TryParse(_configuration["Jwt:AccessTokenExpire"]!, out double expireMinutes);
            var decryptedUsername = _encryptionHelper.Decrypt(user.Username!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, decryptedUsername),
                    new(ClaimTypes.Role, user.Role.Name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
