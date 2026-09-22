using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.Building;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Contracts.Responses.Building;

namespace SIA.SchedulingService.Api.Controllers.Infrastructure;

[Authorize]
[ApiController]
[Route("api/Building")]
public sealed class BuildingController : ControllerBase
{
    private readonly CreateBuildingUseCase _createBuildingUseCase;
    private readonly UpdateBuildingUseCase _updateBuildingUseCase;
    private readonly ActivateBuildingUseCase _activateBuildingUseCase;
    private readonly DeactivateBuildingUseCase _deactivateBuildingUseCase;
    private readonly GetBuildingByIdUseCase _getBuildingByIdUseCase;
    private readonly ITenantContext _tenantContext;

    public BuildingController(
        CreateBuildingUseCase createBuildingUseCase,
        UpdateBuildingUseCase updateBuildingUseCase,
        ActivateBuildingUseCase activateBuildingUseCase,
        DeactivateBuildingUseCase deactivateBuildingUseCase,
        GetBuildingByIdUseCase getBuildingByIdUseCase,
        ITenantContext tenantContext)
    {
        _createBuildingUseCase = createBuildingUseCase;
        _updateBuildingUseCase = updateBuildingUseCase;
        _activateBuildingUseCase = activateBuildingUseCase;
        _deactivateBuildingUseCase = deactivateBuildingUseCase;
        _getBuildingByIdUseCase = getBuildingByIdUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBuildingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBuildingResponse>> CreateAsync([FromBody] CreateBuildingRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createBuildingUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateBuildingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateBuildingResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateBuildingRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateBuildingUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
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
        await _deactivateBuildingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
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
        await _activateBuildingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BuildingDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var response = await _getBuildingByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);
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