using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.AcademicLoad;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;

namespace SIA.SchedulingService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/AcademicLoad")]
public sealed class AcademicLoadController : ControllerBase
{
    private readonly CreateAcademicLoadUseCase _createAcademicLoadUseCase;
    private readonly UpdateAcademicLoadUseCase _updateAcademicLoadUseCase;
    private readonly DeactivateAcademicLoadUseCase _deactivateAcademicLoadUseCase;
    private readonly ActivateAcademicLoadUseCase _activateAcademicLoadUseCase;
    private readonly GetAcademicLoadByIdUseCase _getAcademicLoadByIdUseCase;
    private readonly ITenantContext _tenantContext;


    public AcademicLoadController(
        CreateAcademicLoadUseCase createAcademicLoadUseCase,
        UpdateAcademicLoadUseCase updateAcademicLoadUseCase,
        DeactivateAcademicLoadUseCase deactivateAcademicLoadUseCase,
        ActivateAcademicLoadUseCase activateAcademicLoadUseCase,
        GetAcademicLoadByIdUseCase getAcademicLoadByIdUseCase,
        ITenantContext tenantContext)
    {
        _createAcademicLoadUseCase = createAcademicLoadUseCase;
        _updateAcademicLoadUseCase = updateAcademicLoadUseCase;
        _deactivateAcademicLoadUseCase = deactivateAcademicLoadUseCase;
        _activateAcademicLoadUseCase = activateAcademicLoadUseCase;
        _getAcademicLoadByIdUseCase = getAcademicLoadByIdUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAcademicLoadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateAcademicLoadResponse>> CreateAsync([FromBody] CreateAcademicLoadRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createAcademicLoadUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicLoadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicLoadResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateAcademicLoadRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateAcademicLoadUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
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
        await _deactivateAcademicLoadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
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
        await _activateAcademicLoadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AcademicLoadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicLoadDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
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