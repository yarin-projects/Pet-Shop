using Microsoft.AspNetCore.StaticFiles;

namespace PetShop.Services.Images
{
    public class ImageHanlder : IImageHandler
    {
        public async Task<string> SaveImageToFileAsync(IFormFile image)
        {
            var fileName = Path.GetFileName(image.FileName);
            string? uniqueImageName;
            if (fileName.Length > 55)
                uniqueImageName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            else
                uniqueImageName = Guid.NewGuid().ToString() + "_" + fileName;
            var savePath = "wwwroot/images/" + uniqueImageName;

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return uniqueImageName;
        }
        public IFormFile GetImageFromDir(string pictureName)
        {
            var filePath = "wwwroot/images/" + pictureName;

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IFormFile image = new FormFile(fileStream, 0, fileStream.Length, "", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(filePath)
            };

            return image;
        }
        private static string GetContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(filePath, out contentType!))
            {
                contentType = "application/octet-stream"; //generic file type if no content type could be found
            }
            return contentType;
        }
    }
}
