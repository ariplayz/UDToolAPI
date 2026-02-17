using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UDToolAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "UDToolAPI");

        // Endpoint to upload a file with a specific name
        [HttpPost("{fileName}")]
        public async Task<IActionResult> Upload(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            Directory.CreateDirectory(TempPath);

            var filePath = Path.Combine(TempPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { message = "File uploaded successfully.", filePath });
        }

        // Endpoint to list all files in the temp directory
        [HttpGet("list")]
        public IActionResult List()
        {
            if (!Directory.Exists(TempPath))
                return Ok(new List<string>());

            var fileNames = Directory.GetFiles(TempPath)
                .Select(Path.GetFileName)
                .ToList();
            return Ok(fileNames);
        }

        // Endpoint to download a file by name
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

        // Endpoint to find files containing the search term in their name
        [HttpGet("search/{searchTerm}")]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return BadRequest("Search term is required.");
            var files = Directory.GetFiles(TempPath, $"*{searchTerm}*");
            var fileNames = files.Select(Path.GetFileName).ToArray();
            return Ok(fileNames);
        }

        // Endpoint to delete a file by name
        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            var filePath = Path.Combine(TempPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            System.IO.File.Delete(filePath);

            return Ok(new { message = "File deleted successfully." });
        }
    }
}
