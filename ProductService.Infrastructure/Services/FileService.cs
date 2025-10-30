using ProductService.Domain.Interfaces;

namespace ProductService.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public string UploadFile(byte[] fileBytes, string fileName, string folderName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            string filePath = Path.Combine(folderPath, uniqueFileName);

            File.WriteAllBytes(filePath, fileBytes);

            return Path.Combine("images", folderName, uniqueFileName).Replace("\\", "/");
        }



        public void DeleteFile(string relativeFilePath)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

    }
}
