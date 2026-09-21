using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;
using Microsoft.AspNetCore.Authorization;
using SIA.SchedulingService.Application.Interfaces;

namespace SIA.SchedulingService.Api.Controllers.Proposals;

[Authorize]
[ApiController]
[Route("api/academic-load-proposals")]
public sealed class ProposalsController : ControllerBase
{
  private readonly CreateUseCase _createUseCase;
  private readonly SubmitForReviewUseCase _submitProposalForReviewUseCase;
  private readonly ITenantContext _tenantContext;

    public ProposalsController(
        CreateUseCase createUseCase,
        SubmitForReviewUseCase submitProposalForReviewUseCase,
        ITenantContext tenantContext)
    {
        _createUseCase = createUseCase;
        _submitProposalForReviewUseCase = submitProposalForReviewUseCase;
        _tenantContext = tenantContext;
    }


  [HttpPost]
  [ProducesResponseType(typeof(CreateProposalResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateProposalResponse>> CreateAsync([FromBody] CreateProposalRequest request, CancellationToken cancellationToken)
  {
    var tenantId = _tenantContext.TenantId;
    var correlationId = ResolveCorrelationId();
    Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
    var response = await _createUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, response);
  }

  
    [HttpPost("{proposalId:guid}/submit-for-review")]
    [ProducesResponseType(typeof(SubmitProposalForReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmitProposalForReviewResponse>> SubmitForReviewAsync([FromRoute] Guid proposalId, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _submitProposalForReviewUseCase.ExecuteAsync(tenantId, proposalId, correlationId, cancellationToken);
        return Ok(response);
    }

    private Guid ResolveCorrelationId()
  {
    const string headerName = "X-Correlation-Id";

    if (Request.Headers.TryGetValue(headerName, out var headerValue) && Guid.TryParse(headerValue.FirstOrDefault(), out var correlationId))
    {
      return correlationId;
    }

    return Guid.NewGuid();
  }
}
