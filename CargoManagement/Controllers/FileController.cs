using CargoManagement.Models.Files;
using CargoManagement.Models.Shared;
using CargoManagement.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace CargoManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        #region Property  
        private readonly IFileService _fileService;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly CMSConfig _config;
        #endregion

        #region Constructor  
        public FileController(IFileService fileService, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IOptions<CMSConfig> config)
        {
            _fileService = fileService;
            _hostingEnvironment = hostingEnvironment;
            _config = config.Value;
        }
        #endregion

        [HttpPost("UploadProfilePic")]
        public async Task<ActionResult<ResponseModel>> UploadProfilePic(List<IFormFile> formFiles)
        {
            try
            {
                string filename = _fileService.UploadFile(formFiles[0], _config.ProfileFolder);
                return (new CMSResponse()).Ok(
                    new FileResponseDTO()
                    { Name = filename, Url = _config.FileDownloadUrl + "/" + _config.ProfileFolderAlias + "/" + filename }
                    );
            }
            catch (Exception ex)
            {
                return new CMSResponse().BadRequest((ex.Message));
            }
        }

        [HttpPost("UploadBookingDoc")]
        public async Task<ActionResult<ResponseModel>> UploadBookingDoc(List<IFormFile> formFiles)
        {
            try
            {
                string filename = _fileService.UploadFile(formFiles[0], _config.BookingDocsFolder);
                return (new CMSResponse()).Ok(
                    new FileResponseDTO()
                    { Name = filename, Url = _config.FileDownloadUrl + "/" + _config.BookingDocsFolderAlias + "/" + filename }
                    );
            }
            catch (Exception ex)
            {
                return new CMSResponse().BadRequest((ex.Message));
            }
        }

        [HttpGet("DownloadProfilePic")]
        public async Task<IActionResult> DownloadProfilePic(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, _config.ProfileFolder, fileName);
                var content = await System.IO.File.ReadAllBytesAsync(filePath);
                new FileExtensionContentTypeProvider()
                    .TryGetContentType(fileName, out string contentType);
                return File(content, contentType, fileName);
            }
            catch
            {
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpGet("DownloadBookingDoc")]
        public async Task<IActionResult> DownloadBookingDoc(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, _config.BookingDocsFolder, fileName);
                var content = await System.IO.File.ReadAllBytesAsync(filePath);
                new FileExtensionContentTypeProvider()
                    .TryGetContentType(fileName, out string contentType);
                return File(content, contentType, fileName);
            }
            catch
            {
                return BadRequest();
            }

            return BadRequest();
        }

    }
}
