namespace CargoManagement.Services.Abstractions
{
    public interface IFileService
    {
        string UploadFile(IFormFile file, string subDirectory);
    }
}
