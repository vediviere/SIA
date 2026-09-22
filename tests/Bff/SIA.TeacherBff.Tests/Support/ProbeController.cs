using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.TeacherBff.Configuration;
using SIA.TeacherBff.Infrastructure.Errors;
using SIA.TeacherBff.Infrastructure.Tenancy;

namespace SIA.TeacherBff.Tests.Support;

[ApiController]
[Authorize]
[Route("api/probe")]
public sealed class ProbeController(ITenantContext tenantContext) : ControllerBase
{
  [HttpGet]
  public ActionResult<ContextDto> Get()
  {
    return Ok(new ContextDto(tenantContext.TenantId));
  }

  [HttpGet("missing")]
  public IActionResult Missing()
  {
    throw new InternalException(ServiceConfig.Scheduling, HttpStatusCode.NotFound);
  }
}

public sealed record ContextDto(Guid TenantId);
