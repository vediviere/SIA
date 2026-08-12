using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.Building;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Contracts.Responses.Building;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/Building")]
public sealed class BuildingController : ControllerBase
{
    private readonly CreateBuildingUseCase _createBuildingUseCase;
    private readonly UpdateBuildingUseCase _updateBuildingUseCase;
    private readonly ActivateBuildingUseCase _activateBuildingUseCase;
    private readonly DeactivateBuildingUseCase _deactivateBuildingUseCase;
    private readonly GetBuildingByIdUseCase _getBuildingByIdUseCase;

    public BuildingController(
        CreateBuildingUseCase createBuildingUseCase,
        UpdateBuildingUseCase updateBuildingUseCase,
        ActivateBuildingUseCase activateBuildingUseCase,
        DeactivateBuildingUseCase deactivateBuildingUseCase,
        GetBuildingByIdUseCase getBuildingByIdUseCase)
    {
        _createBuildingUseCase = createBuildingUseCase;
        _updateBuildingUseCase = updateBuildingUseCase;
        _activateBuildingUseCase = activateBuildingUseCase;
        _deactivateBuildingUseCase = deactivateBuildingUseCase;
        _getBuildingByIdUseCase = getBuildingByIdUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBuildingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBuildingResponse>> CreateAsync([FromBody] CreateBuildingRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createBuildingUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateBuildingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateBuildingResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateBuildingRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateBuildingUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateBuildingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _activateBuildingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BuildingDto>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
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