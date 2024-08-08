namespace PetShop.Services.Encryption.Argon2Hashing
{
    public interface IArgon2PasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
