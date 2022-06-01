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

        public FileManageController(IUploaderService uploaderService)
        {
            _uploaderService = uploaderService;
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFileAsync([FromBody] FileUploaded fileUploaded)
            => Ok(await _uploaderService.UploadFileAsync(fileUploaded));
    }
}
