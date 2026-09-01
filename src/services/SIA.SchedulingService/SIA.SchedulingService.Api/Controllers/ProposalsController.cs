using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;

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

  [HttpPost]
  [ProducesResponseType(typeof(CreateProposalResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateProposalResponse>> CreateAsync([FromBody] CreateProposalRequest request, CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();
    Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

    var response = await _createUseCase.ExecuteAsync(request, correlationId, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, response);
  }


    [HttpPost("{proposalId:guid}/submit-for-review")]
    [ProducesResponseType(typeof(SubmitProposalForReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmitProposalForReviewResponse>> SubmitForReviewAsync([FromRoute] Guid proposalId, [FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
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
