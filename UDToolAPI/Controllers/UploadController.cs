using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UDToolAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "UDToolAPI");

        [HttpPost("{fileName}")]
        public async Task<IActionResult> Upload(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Ensure the directory exists
            Directory.CreateDirectory(TempPath);

            var filePath = Path.Combine(TempPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { message = "File uploaded successfully.", filePath });
        }

        [HttpGet("{fileName}")]
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            var filePath = Path.Combine(TempPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
