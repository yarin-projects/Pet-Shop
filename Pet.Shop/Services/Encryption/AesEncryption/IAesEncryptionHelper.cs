namespace PetShop.Services.Encryption.AesEncryption
{
    public interface IAesEncryptionHelper
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
