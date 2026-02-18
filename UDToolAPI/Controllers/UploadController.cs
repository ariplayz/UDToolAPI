using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UDToolAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "UDToolAPI");
        private static readonly string KeysPath = Path.Combine(TempPath, "keys.txt");

        private string GetKeyPath(string key)
        {
            return Path.Combine(TempPath, key);
        }

        [HttpGet("/key/check/{key}")]
        public IActionResult GetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");
            
            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    return Ok(new { message = "Key is valid.", key });
                }
                else
                {
                    return Ok(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Ok(new { message = "Key is not valid.", key });
            }
        }

        [HttpPost("/key/new")]
        public IActionResult CreateKey()
        {
            var newKey = Guid.NewGuid().ToString();
            System.IO.File.AppendAllLines(KeysPath, new[] { newKey });
            return Ok(new { message = "New key created successfully.", key = newKey });
        }

        // Endpoint to upload a file with a specific name
        [HttpPost("{key}/{fileName}")]
        public async Task<IActionResult> Upload(IFormFile file, string key, string fileName)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");

            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    if (file == null || file.Length == 0)
                        return BadRequest("No file uploaded.");

                    var keyPath = GetKeyPath(key);
                    Directory.CreateDirectory(keyPath);

                    var filePath = Path.Combine(keyPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return Ok(new { message = "File uploaded successfully.", filePath });
                }
                else
                {
                    return Unauthorized(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Unauthorized(new { message = "Key is not valid.", key });
            }
        }

        // Endpoint to list all files in the temp directory
        [HttpGet("{key}/list")]
        public IActionResult List(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");

            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    var keyPath = GetKeyPath(key);
                    if (!Directory.Exists(keyPath))
                        return Ok(new List<string>());

                    var fileNames = Directory.GetFiles(keyPath)
                        .Select(Path.GetFileName)
                        .ToList();
                    return Ok(fileNames);
                }
                else
                {
                    return Unauthorized(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Unauthorized(new { message = "Key is not valid.", key });
            }
        }

        // Endpoint to download a file by name
        [HttpGet("{key}/{fileName}")]
        public IActionResult Download(string key, string fileName)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");

            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    var keyPath = GetKeyPath(key);
                    var filePath = Path.Combine(keyPath, fileName);

                    if (!System.IO.File.Exists(filePath))
                        return NotFound("File not found.");

                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", fileName);
                }
                else
                {
                    return Unauthorized(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Unauthorized(new { message = "Key is not valid.", key });
            }
        }

        // Endpoint to find files containing the search term in their name
        [HttpGet("{key}/search/{searchTerm}")]
        public IActionResult Search(string key, string searchTerm)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");

            if (string.IsNullOrEmpty(searchTerm))
                return BadRequest("Search term is required.");

            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    var keyPath = GetKeyPath(key);
                    var files = Directory.GetFiles(keyPath, $"*{searchTerm}*");
                    var fileNames = files.Select(Path.GetFileName).ToArray();
                    return Ok(fileNames);
                }
                else
                {
                    return Unauthorized(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Unauthorized(new { message = "Key is not valid.", key });
            }
        }

        // Endpoint to delete a file by name
        [HttpDelete("{key}/{fileName}")]
        public IActionResult Delete(string key, string fileName)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key is required.");

            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            if (System.IO.File.Exists(KeysPath))
            {
                var keys = System.IO.File.ReadAllLines(KeysPath);
                if (keys.Contains(key))
                {
                    var keyPath = GetKeyPath(key);
                    var filePath = Path.Combine(keyPath, fileName);

                    if (!System.IO.File.Exists(filePath))
                        return NotFound("File not found.");

                    System.IO.File.Delete(filePath);

                    return Ok(new { message = "File deleted successfully." });
                }
                else
                {
                    return Unauthorized(new { message = "Key is not valid.", key });
                }
            }
            else
            {
                System.IO.File.Create(KeysPath).Dispose();
                return Unauthorized(new { message = "Key is not valid.", key });
            }
        }
    }
}
