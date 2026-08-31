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

  public ProposalsController(CreateProposalUseCase createUseCase)
  {
    _createUseCase = createUseCase;
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
