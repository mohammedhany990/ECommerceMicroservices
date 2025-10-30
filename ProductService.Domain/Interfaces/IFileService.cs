namespace ProductService.Domain.Interfaces
{
    public interface IFileService
    {
        string UploadFile(byte[] fileBytes, string fileName, string folderName);
        void DeleteFile(string relativeFilePath);
    }
}
