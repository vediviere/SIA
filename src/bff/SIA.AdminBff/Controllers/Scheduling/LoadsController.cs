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
public sealed class LoadsController : ControllerBase
{
  private readonly ISchedulingClient _schedulingClient;
  private readonly ITenantContext _tenantContext;

  public LoadsController(ISchedulingClient schedulingClient, ITenantContext tenantContext)
  {
    _schedulingClient = schedulingClient;
    _tenantContext = tenantContext;
  }

  [HttpPost("proposals/{proposalId:guid}/loads")]
  [ProducesResponseType(typeof(LoadResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<LoadResponse>> CreateAsync([FromRoute] Guid proposalId, [FromBody] CreateLoadRequest request, CancellationToken cancellationToken)
  {
    var dto = new LoadCreateDto
    {
      TenantId = _tenantContext.TenantId,
      ProposalId = proposalId,
      TeacherId = request.TeacherId,
      DivisionId = request.DivisionId,
      AcademicPeriodId = request.AcademicPeriodId,
      OfficialLetterNumber = request.OfficialLetterNumber,
      ProposedDate = request.ProposedDate,
      AssignmentDate = request.AssignmentDate
    };

    var response = await _schedulingClient.CreateLoadAsync(dto, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, LoadResponse.FromDto(response));
  }

  [HttpPut("loads/{loadId:guid}")]
  [ProducesResponseType(typeof(LoadResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<LoadResponse>> UpdateAsync([FromRoute] Guid loadId, [FromBody] UpdateLoadRequest request, CancellationToken cancellationToken)
  {
    var dto = new LoadUpdateDto
    {
      OfficialLetterNumber = request.OfficialLetterNumber,
      ProposedDate = request.ProposedDate,
      AssignmentDate = request.AssignmentDate
    };

    var response = await _schedulingClient.UpdateLoadAsync(_tenantContext.TenantId, loadId, dto, cancellationToken);

    return Ok(LoadResponse.FromDto(response));
  }
}
