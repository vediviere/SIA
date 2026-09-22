using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.TeachingSupportHours;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;

namespace SIA.SchedulingService.Api.Controllers.Support;

[Authorize]
[ApiController]
[Route("api/TeachingSupportHours")]

public sealed class TeachingSupportHoursController : ControllerBase
{
    private readonly CreateTeachingSupportHoursUseCase _createTeachingSupportHoursUseCase;
    private readonly UpdateTeachingSupportHoursUseCase _updateTeachingSupportHoursUseCase;
    private readonly DeactivateTeachingSupportHoursUseCase _deactivateTeachingSupportHoursUseCase;
    private readonly ActivateTeachingSupportHoursUseCase _activateTeachingSupportHoursUseCase;
    private readonly GetTeachingSupportHoursByIdUseCase _getTeachingSupportHoursByIdUseCase;
    private readonly ITenantContext _tenantContext;


    public TeachingSupportHoursController(
        CreateTeachingSupportHoursUseCase createTeachingSupportHoursUseCase,
        UpdateTeachingSupportHoursUseCase updateTeachingSupportHoursUseCase,
        DeactivateTeachingSupportHoursUseCase deactivateTeachingSupportHoursUseCase,
        ActivateTeachingSupportHoursUseCase activateTeachingSupportHoursUseCase,
        GetTeachingSupportHoursByIdUseCase getTeachingSupportHoursByIdUseCase,
        ITenantContext tenantContext)
    {
        _createTeachingSupportHoursUseCase = createTeachingSupportHoursUseCase;
        _updateTeachingSupportHoursUseCase = updateTeachingSupportHoursUseCase;
        _deactivateTeachingSupportHoursUseCase = deactivateTeachingSupportHoursUseCase;
        _activateTeachingSupportHoursUseCase = activateTeachingSupportHoursUseCase;
        _getTeachingSupportHoursByIdUseCase = getTeachingSupportHoursByIdUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTeachingSupportHoursResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateTeachingSupportHoursResponse>> CreateAsync([FromBody] CreateTeachingSupportHoursRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createTeachingSupportHoursUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTeachingSupportHoursResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateTeachingSupportHoursResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateTeachingSupportHoursRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateTeachingSupportHoursUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
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
        await _deactivateTeachingSupportHoursUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
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
        await _activateTeachingSupportHoursUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeachingSupportHoursDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeachingSupportHoursDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var response = await _getTeachingSupportHoursByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);
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
