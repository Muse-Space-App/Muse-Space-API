using Microsoft.AspNetCore.Mvc;

namespace MuseSpace.API.Controllers;

/// <summary>
/// Template
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TemplateController : ControllerBase
{
    /// <summary>
    /// Tenplate
    /// </summary>
    /// <returns></returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { message = "MuseSpace API is running." });
    }
}
