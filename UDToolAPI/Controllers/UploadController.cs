using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UDToolAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "UDToolAPI");

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Ensure the directory exists
            Directory.CreateDirectory(TempPath);

            var filePath = Path.Combine(TempPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Here you can add code to process the uploaded file as needed
            return Ok(new { message = "File uploaded successfully.", filePath });
        }

        [HttpGet]
        public async Task<IActionResult> Download()
        {
            if (!Directory.Exists(TempPath))
                return NotFound("No files directory found.");

            var files = Directory.GetFiles(TempPath);
            return Ok(files);
        }
    }
}
