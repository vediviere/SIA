using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Contracts.Scheduling.Requests;
using SIA.AdminBff.Contracts.Scheduling.Responses;
using SIA.AdminBff.Infrastructure.Errors;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Controllers.Scheduling;

[ApiController]
[Authorize]
[Route("api/academic-planning")]
public sealed class SupportHoursController : ControllerBase
{
  private readonly ISchedulingClient _schedulingClient;
  private readonly ITenantContext _tenantContext;

  public SupportHoursController(ISchedulingClient schedulingClient, ITenantContext tenantContext)
  {
    _schedulingClient = schedulingClient;
    _tenantContext = tenantContext;
  }

  [HttpPost("loads/{loadId:guid}/support-hours")]
  [ProducesResponseType(typeof(SupportHourResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<SupportHourResponse>> CreateAsync([FromRoute] Guid loadId, [FromBody] CreateSupportHoursRequest request, CancellationToken cancellationToken)
  {
    var dto = new SupportHourCreateDto
    {
      TenantId = _tenantContext.TenantId,
      ActivityId = request.ActivityId,
      AcademicLoadId = loadId,
      Hours = request.Hours
    };

    var response = await _schedulingClient.CreateSupportHourAsync(dto, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, SupportHourResponse.FromDto(response));
  }

  [HttpPut("support-hours/{supportHourId:guid}")]
  [ProducesResponseType(typeof(SupportHourResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<SupportHourResponse>> UpdateAsync([FromRoute] Guid supportHourId, [FromBody] UpdateSupportHoursRequest request, CancellationToken cancellationToken)
  {
    var dto = new SupportHourUpdateDto
    {
      Hours = request.Hours
    };

    var response = await _schedulingClient.UpdateSupportHourAsync(_tenantContext.TenantId, supportHourId, dto, cancellationToken);

    return Ok(SupportHourResponse.FromDto(response));
  }
}
