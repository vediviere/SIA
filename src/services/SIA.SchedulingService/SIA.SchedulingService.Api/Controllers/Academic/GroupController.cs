using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.Group;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Contracts.Responses.Group;

namespace SIA.SchedulingService.Api.Controllers.Academic;

[Authorize]
[ApiController]
[Route("api/Groups")]

public sealed class GroupController : ControllerBase
{
    private readonly CreateGroupUseCase _createGroupUseCase;
    private readonly UpdateGroupUseCase _updateGroupUseCase;
    private readonly DeactivateGroupUseCase _deactivateGroupUseCase;
    private readonly ActivateGroupUseCase _activateGroupUseCase;
    private readonly GetGroupByIdUseCase _getGroupByIdUseCase;
    private readonly ITenantContext _tenantContext;

    public GroupController(
        CreateGroupUseCase createGroupUseCase,
        UpdateGroupUseCase updateGroupUseCase,
        DeactivateGroupUseCase deactivateGroupUseCase,
        ActivateGroupUseCase activateGroupUseCase,
        GetGroupByIdUseCase getGroupByIdUseCase,
        ITenantContext tenantContext
        )
    {
        _createGroupUseCase = createGroupUseCase;
        _updateGroupUseCase = updateGroupUseCase;
        _deactivateGroupUseCase = deactivateGroupUseCase;
        _activateGroupUseCase = activateGroupUseCase;
        _getGroupByIdUseCase = getGroupByIdUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateGroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateGroupResponse>> CreateAsync([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createGroupUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateGroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateGroupResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateGroupUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateGroupUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _activateGroupUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
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
