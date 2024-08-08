using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System.Security.Cryptography;
using System.Text;

namespace PetShop.Services.Encryption.Argon2Hashing
{
    public class Argon2PasswordHasher : IArgon2PasswordHasher
    {
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var config = new Argon2Config
            {
                Type = Argon2Type.HybridAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10, // Number of iterations
                MemoryCost = 32768, // 32 MB
                Lanes = 5, // Number of threads and lanes
                Threads = Environment.ProcessorCount, // Number of threads
                Salt = salt,
                Password = Encoding.UTF8.GetBytes(password),
                HashLength = 32 // Length of the hash in bytes
            };
            var argon2 = new Argon2(config);
            using SecureArray<byte> hash = argon2.Hash();
            return config.EncodeString(hash.Buffer);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return Argon2.Verify(hashedPassword, providedPassword, 5);
        }
    }
}
