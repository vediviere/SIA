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
[Route("api/academic-planning/proposals")]
public sealed class ProposalsController : ControllerBase
{
  private readonly ISchedulingClient _schedulingClient;
  private readonly ITenantContext _tenantContext;

  public ProposalsController(ISchedulingClient schedulingClient, ITenantContext tenantContext)
  {
    _schedulingClient = schedulingClient;
    _tenantContext = tenantContext;
  }

  [HttpPost]
  [ProducesResponseType(typeof(ProposalResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<ProposalResponse>> CreateAsync([FromBody] CreateProposalRequest request, CancellationToken cancellationToken)
  {
    var response = await _schedulingClient.CreateProposalAsync(
        _tenantContext.TenantId,
        request.EducationalProgramId,
        request.AcademicPeriodId,
        request.DivisionHeadId,
        cancellationToken);

    return StatusCode(StatusCodes.Status201Created, ProposalResponse.FromService(response));
  }

  [HttpPost("{proposalId:guid}/submit-for-review")]
  [ProducesResponseType(typeof(ProposalResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<ProposalResponse>> SubmitForReviewAsync([FromRoute] Guid proposalId, CancellationToken cancellationToken)
  {
    var response = await _schedulingClient.SubmitForReviewAsync(_tenantContext.TenantId, proposalId, cancellationToken);

    return Ok(ProposalResponse.FromService(response));
  }
}
