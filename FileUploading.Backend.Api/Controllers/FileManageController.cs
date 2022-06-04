using FileUploading.Backend.Core.Interfaces;
using FileUploading.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploading.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManageController : ControllerBase
    {
        private readonly IUploaderService _uploaderService;
        private readonly IConfiguration _configuration;

        public FileManageController(IUploaderService uploaderService, IConfiguration configuration)
        {
            _uploaderService = uploaderService;
            _configuration = configuration;
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFileAsync([FromBody] FileUploaded fileUploaded)
            => Ok(await _uploaderService.UploadFileAsync(fileUploaded));

        [HttpGet("list-files")]
        public async Task<IActionResult> GetUploadedFilesAsync()
            => Ok(await _uploaderService.GetUploadedFilesAsync());

        [HttpPost("upload-blob-file")]
        public async Task<IActionResult> UploadBlobAsync([FromQuery] string id, [FromForm] IFormFile file)
            => Ok(await _uploaderService.UploadFileToAzureAsync(id, file, _configuration.GetConnectionString("AzBlobConnectionString")));
    }
}
