using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Workflow;
using SIA.AdminBff.Infrastructure.Errors;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Controllers.Workflow;

[ApiController]
[Authorize]
[Route("api/review-processes")]
public sealed class WorkflowReviewsController : ControllerBase
{
    private readonly IWorkflowClient _workflowClient;
    private readonly ITenantContext _tenantContext;

    public WorkflowReviewsController(IWorkflowClient workflowClient, ITenantContext tenantContext)
    {
        _workflowClient = workflowClient;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewProcessListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<ReviewProcessListItemDto>>> GetPendingReviewsAsync(CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var result = await _workflowClient.GetPendingReviewsAsync(tenantId, cancellationToken);

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
        var tenantId = _tenantContext.TenantId;
        var result = await _workflowClient.GetReviewByIdAsync(tenantId, processId, cancellationToken);

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
        var tenantId = _tenantContext.TenantId;
        await _workflowClient.ApproveReviewAsync(tenantId, processId, cancellationToken);

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
        var tenantId = _tenantContext.TenantId;
        await _workflowClient.ReturnReviewAsync(tenantId, processId, request, cancellationToken);

        return NoContent();
    }
}