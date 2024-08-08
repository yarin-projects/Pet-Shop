using System.Security.Cryptography;

namespace PetShop.Services.Encryption.AesEncryption
{
    public class AesEncryptionHelper(IConfiguration configuration) : IAesEncryptionHelper
    {
        private readonly IConfiguration _configuration = configuration;
        private const int AesBlockSize = 128; // 128 bits

        public string Encrypt(string plainText)
        {
            byte[] key = Convert.FromBase64String(_configuration["Aes:Key"]!);
            byte[] iv;
            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.BlockSize = AesBlockSize;
                aes.GenerateIV();
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                encrypted = ms.ToArray();
            }
            // Combine IV and encrypted data
            byte[] combinedIvEnc = new byte[iv.Length + encrypted.Length];
            Array.Copy(iv, 0, combinedIvEnc, 0, iv.Length);
            Array.Copy(encrypted, 0, combinedIvEnc, iv.Length, encrypted.Length);

            return Convert.ToBase64String(combinedIvEnc);
        }

        public string Decrypt(string cipherText)
        {
            byte[] combinedIvEnc = Convert.FromBase64String(cipherText);
            byte[] key = Convert.FromBase64String(_configuration["Aes:Key"]!);
            byte[] iv = new byte[AesBlockSize / 8];
            byte[] encrypted = new byte[combinedIvEnc.Length - iv.Length];

            Array.Copy(combinedIvEnc, 0, iv, 0, iv.Length);
            Array.Copy(combinedIvEnc, iv.Length, encrypted, 0, encrypted.Length);

            string plaintext;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using var ms = new MemoryStream(encrypted);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                plaintext = sr.ReadToEnd();
            }

            return plaintext;
        }
    }
}
