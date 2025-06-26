using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    //private readonly ILogger<TestController> _logger;

    public TestController()//ILogger<TestController> logger)
    {
        //_logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new List<string>() { "aaa" };
    }
}