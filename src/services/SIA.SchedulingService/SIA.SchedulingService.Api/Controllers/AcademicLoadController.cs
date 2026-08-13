using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.AcademicLoad;
using SIA.SchedulingService.Application.DTOs.Building;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.Building;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/AcademicLoad")]
public sealed class AcademicLoadController : ControllerBase
{
    private readonly CreateAcademicLoadUseCase _createAcademicLoadUseCase;
    private readonly UpdateAcademicLoadUseCase _updateAcademicLoadUseCase;
    private readonly DeactivateAcademicLoadUseCase _deactivateAcademicLoadUseCase;
    private readonly ActivateAcademicLoadUseCase _activateAcademicLoadUseCase;
    private readonly GetAcademicLoadByIdUseCase _getAcademicLoadByIdUseCase;


    public AcademicLoadController(
        CreateAcademicLoadUseCase createAcademicLoadUseCase,
        UpdateAcademicLoadUseCase updateAcademicLoadUseCase,
        DeactivateAcademicLoadUseCase deactivateAcademicLoadUseCase,
        ActivateAcademicLoadUseCase activateAcademicLoadUseCase,
        GetAcademicLoadByIdUseCase getAcademicLoadByIdUseCase)
    {
        _createAcademicLoadUseCase = createAcademicLoadUseCase;
        _updateAcademicLoadUseCase = updateAcademicLoadUseCase;
        _deactivateAcademicLoadUseCase = deactivateAcademicLoadUseCase;
        _activateAcademicLoadUseCase = activateAcademicLoadUseCase;
        _getAcademicLoadByIdUseCase = getAcademicLoadByIdUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAcademicLoadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateAcademicLoadResponse>> CreateAsync([FromBody] CreateAcademicLoadRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createAcademicLoadUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicLoadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicLoadResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateAcademicLoadRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateAcademicLoadUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateAcademicLoadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _activateAcademicLoadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(AcademicLoadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicLoadDto>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _getAcademicLoadByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);
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