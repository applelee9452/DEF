using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DEF.CustomerSvc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IWebHostEnvironment env;

    public FileController(IWebHostEnvironment environment)
    {
        this.env = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        var filePath = $"upload\\{file.FileName}";
        var path = $"{env.WebRootPath}\\{filePath}";

        using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);

        return Ok(new
        {
            filePath
        });
    }
}