namespace PetShop.Services.Images
{
    public interface IImageHandler
    {
        Task<string> SaveImageToFileAsync(IFormFile image);
        IFormFile GetImageFromDir(string pictureName);
    }
}
