using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SIA.AcademicStaffService.Api.Controllers.HealthController;

[AllowAnonymous]
[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Check()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}