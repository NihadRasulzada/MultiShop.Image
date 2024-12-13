using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Images.Services;

namespace MultiShop.Images.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;
        public ImageController(ICloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null)
            {
                var fileNameToSave = GenerateFileNameToSave(file.FileName);
                var url = await _cloudStorageService.UploadFileAsync(file, fileNameToSave);
                return Ok(fileNameToSave);
            }
            else
            {
                return BadRequest("File is null");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImageLink(string fileName)
        {
            var fileUrl = await _cloudStorageService.GetSignedUrlAsync(fileName);
            return Ok(fileUrl);
        }

        [HttpDelete("DeleteImage")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            await _cloudStorageService.DeleteFileAsync(fileName);
            return Ok();
        }

        private string? GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{extension}";
        }
    }
}
