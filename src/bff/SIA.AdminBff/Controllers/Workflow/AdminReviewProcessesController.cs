using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Workflow;
using SIA.AdminBff.Infrastructure.Errors;

namespace SIA.AdminBff.Controllers.Workflow;

[ApiController]
[Authorize]
[Route("api/review-processes")]
public sealed class AdminReviewProcessesController : ControllerBase
{
    private readonly IWorkflowClient _workflowClient;

    public AdminReviewProcessesController(IWorkflowClient workflowClient)
    {
        _workflowClient = workflowClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewProcessListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<ReviewProcessListItemDto>>> GetPendingReviewsAsync(CancellationToken cancellationToken)
    {
        var result = await _workflowClient.GetPendingReviewsAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{processId:guid}")]
    [ProducesResponseType(typeof(ReviewProcessDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ReviewProcessDetailDto>> GetByIdAsync([FromRoute] Guid processId, CancellationToken cancellationToken)
    {
        var result = await _workflowClient.GetReviewByIdAsync(processId, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{processId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ApproveAsync([FromRoute] Guid processId, CancellationToken cancellationToken)
    {
        await _workflowClient.ApproveReviewAsync(processId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{processId:guid}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ReturnAsync(
        [FromRoute] Guid processId,
        [FromBody] ReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        await _workflowClient.ReturnReviewAsync(processId, request, cancellationToken);

        return NoContent();
    }
}