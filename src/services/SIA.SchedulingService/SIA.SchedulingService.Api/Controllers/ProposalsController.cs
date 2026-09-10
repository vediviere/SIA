using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/academic-load-proposals")]
public sealed class ProposalsController : ControllerBase
{
  private readonly CreateProposalUseCase _createUseCase;
  private readonly SubmitProposalForReviewUseCase _submitProposalForReviewUseCase;

    public ProposalsController(
        CreateProposalUseCase createUseCase,
        SubmitProposalForReviewUseCase submitProposalForReviewUseCase)
    {
        _createUseCase = createUseCase;
        _submitProposalForReviewUseCase = submitProposalForReviewUseCase;
    }

  [Authorize]
  [HttpPost]
  [ProducesResponseType(typeof(CreateProposalResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateProposalResponse>> CreateAsync([FromBody] CreateProposalRequest request, CancellationToken cancellationToken)
  {
    var tenantId = ResolveTenantId();
    var correlationId = ResolveCorrelationId();
    Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

    var response = await _createUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, response);
  }

    [Authorize]
    [HttpPost("{proposalId:guid}/submit-for-review")]
    [ProducesResponseType(typeof(SubmitProposalForReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmitProposalForReviewResponse>> SubmitForReviewAsync([FromRoute] Guid proposalId, CancellationToken cancellationToken)
    {
        var tenantId = ResolveTenantId();
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _submitProposalForReviewUseCase.ExecuteAsync(tenantId, proposalId, correlationId, cancellationToken);
        return Ok(response);
    }

    private Guid ResolveTenantId()
    {
        const string tenantClaimType = "tenant_id";

        var tenantIdClaim = User.FindFirst(tenantClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            throw new InvalidOperationException("El token no contiene un tenant_id válido.");
        }

        return tenantId;
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
