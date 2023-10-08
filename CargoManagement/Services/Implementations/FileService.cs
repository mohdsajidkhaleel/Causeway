using CargoManagement.Services.Abstractions;

namespace CargoManagement.Services.Implementations
{
    public class FileService : IFileService
    {
        #region Property  
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructor  
        public FileService(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion
        public string UploadFile(IFormFile file, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);

            Directory.CreateDirectory(target);
            if (file.Length <= 0) return null;
            Guid id = Guid.NewGuid();
            var newFilename = id.ToString() + file.FileName.ToString();
            var filePath = Path.Combine(target, newFilename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return newFilename;

        }
    }
}
