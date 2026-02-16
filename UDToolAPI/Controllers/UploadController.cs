using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UDToolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
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
            var files = Directory.GetFiles(Path.GetTempPath());
            return Ok(files);

        }
    }
}
