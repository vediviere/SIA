using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.IdentityService.Contracts.Enums;
using SIA.WorkflowService.Api.Extensions;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Contracts.Requests.ReviewProcesses;
using SIA.WorkflowService.Application.Interfaces;
using SIA.WorkflowService.Application.Interfaces.Queries;
using SIA.WorkflowService.Contracts.Responses.ReviewProcesses;
using DomainObservationTarget = SIA.WorkflowService.Domain.Enums.ObservationTarget;


namespace SIA.WorkflowService.Api.Controllers;

[ApiController]
[Route("api/review-processes")]
[Authorize(Roles = nameof(RoleCode.Administrator))]
public sealed class ReviewProcessesController : ControllerBase
{
    private readonly ApproveUseCase _approveUseCase;
    private readonly ReturnUseCase _returnUseCase;
    private readonly ITenantContext _tenantContext;
    private readonly IReviewProcessQueries _reviewProcessQueries;

    public ReviewProcessesController(
        ApproveUseCase approveUseCase,
        ReturnUseCase returnUseCase,
        ITenantContext tenantContext,
        IReviewProcessQueries reviewProcessQueries
    )
    {
        _approveUseCase = approveUseCase;
        _returnUseCase = returnUseCase;
        _tenantContext = tenantContext;
        _reviewProcessQueries = reviewProcessQueries;
    }

    [HttpPost("{processId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] Guid processId, 
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var command = new ApproveCommand
        {
            ProcessId = processId,
            TenantId = _tenantContext.TenantId,
            DecidedBy = User.GetUserId(),
            CorrelationId = correlationId
        };

        await _approveUseCase.ExecuteAsync(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{processId:guid}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnAsync(
        [FromRoute] Guid processId,
        [FromBody] ReturnRequest request, 
        CancellationToken cancellationToken
    )
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var observations = request.Observations?.Select(observation => new ObservationInput
        {
            TargetType = (DomainObservationTarget)observation.TargetType,
            TargetId = observation.TargetId,
            Description = observation.Description
        }).ToArray() ?? [];

        var command = new ReturnCommand
        {
            ProcessId = processId,
            TenantId = _tenantContext.TenantId,
            DecidedBy = User.GetUserId(),
            CorrelationId = correlationId,
            Observations = observations
        };

        await _returnUseCase.ExecuteAsync(command, cancellationToken);

        return NoContent();
    }


    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewProcessListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInReviewAsync(CancellationToken cancellationToken)
    {
        var processes = await _reviewProcessQueries.GetInReviewAsync(
            _tenantContext.TenantId,
            cancellationToken);

        return Ok(processes);
    }

    [HttpGet("{processId:guid}")]
    [ProducesResponseType(typeof(ReviewProcessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid processId,
        CancellationToken cancellationToken)
    {
        var process = await _reviewProcessQueries.GetByIdAsync(
            _tenantContext.TenantId,
            processId,
            cancellationToken);

        if (process is null)
        {
            return NotFound();
        }

        return Ok(process);
    }

    private Guid ResolveCorrelationId()
    {
        const string headerName = "X-Correlation-Id";

        if (Request.Headers.TryGetValue(headerName, out var value) && Guid.TryParse(value.FirstOrDefault(), out var correlationId))
        {
            return correlationId;
        }

        return Guid.NewGuid();
    }
}
