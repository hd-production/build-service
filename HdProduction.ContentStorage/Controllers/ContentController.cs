using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HdProduction.ContentStorage.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HdProduction.ContentStorage.Controllers
{
  [Route("")]
  public class ContentController : ControllerBase
  {
    private static readonly string[] SupportedExtensions = {".zip"};
    private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "cnt");

    static ContentController()
    {
      if (!Directory.Exists(BasePath))
      {
        Directory.CreateDirectory(BasePath);
      }
    }

    [HttpGet("{fileKey}")]
    public IActionResult DownloadFile(string fileKey)
    {
      try
      {
        var fileStream = System.IO.File.OpenRead(Path.Combine(BasePath, fileKey));
        return File(fileStream, "application/zip", fileKey);
      }
      catch (FileNotFoundException)
      {
        return NotFound();
      }
    }

    [HttpPost(""), Protected]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
      var fileName = file.FileName;
      if (!SupportedExtensions.Contains(Path.GetExtension(fileName)))
      {
        return BadRequest("Unsupported file format");
      }
      using (var fileStream = new FileStream(Path.Combine(BasePath, fileName), FileMode.Create))
      {
        await file.CopyToAsync(fileStream);
      }

      return Ok(fileName);
    }
  }
}