using Microsoft.AspNetCore.Mvc;

namespace MuseSpace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TemplateController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { message = "MuseSpace API is running." });
    }
}
