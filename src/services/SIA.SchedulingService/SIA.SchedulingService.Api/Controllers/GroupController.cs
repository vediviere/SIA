using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.Group;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Contracts.Responses.Group;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/Groups")]

public sealed class GroupController : ControllerBase
{
    private readonly CreateGroupUseCase _createGroupUseCase;
    private readonly UpdateGroupUseCase _updateGroupUseCase;
    private readonly DeactivateGroupUseCase _deactivateGroupUseCase;
    private readonly ActivateGroupUseCase _activateGroupUseCase;
    private readonly GetGroupByIdUseCase _getGroupByIdUseCase;

    public GroupController(
        CreateGroupUseCase createGroupUseCase,
        UpdateGroupUseCase updateGroupUseCase,
        DeactivateGroupUseCase deactivateGroupUseCase,
        ActivateGroupUseCase activateGroupUseCase,
        GetGroupByIdUseCase getGroupByIdUseCase
        )
    {
        _createGroupUseCase = createGroupUseCase;
        _updateGroupUseCase = updateGroupUseCase;
        _deactivateGroupUseCase = deactivateGroupUseCase;
        _activateGroupUseCase = activateGroupUseCase;
        _getGroupByIdUseCase = getGroupByIdUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateGroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateGroupResponse>> CreateAsync([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createGroupUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateGroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateGroupResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateGroupUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateGroupUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _activateGroupUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _getGroupByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);
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