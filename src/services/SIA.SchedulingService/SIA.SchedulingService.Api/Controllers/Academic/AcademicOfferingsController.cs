using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.AcademicOffering;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Contracts.Responses;
using SIA.SchedulingService.Contracts.Responses.AcademicOffering;

namespace SIA.SchedulingService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/AcademicOffering")]
public class AcademicOfferingsController : ControllerBase
{
    private readonly CreateAcademicOfferingUseCase _createAcademicOfferingUseCase;
    private readonly UpdateAcademicOfferingUseCase _updateAcademicOfferingUseCase;
    private readonly DeactivateAcademicOfferingUseCase _deactivateAcademicOfferingUseCase;
    private readonly ActivateAcademicOfferingUseCase _activateAcademicOfferingUseCase;
    private readonly GetAcademicOfferingByIdUseCase _getAcademicOfferingByIdUseCase;
    private readonly ITenantContext _tenantContext;

    public AcademicOfferingsController(
        CreateAcademicOfferingUseCase createAcademicOfferingUseCase, 
        UpdateAcademicOfferingUseCase updateAcademicOfferingUseCase,
        DeactivateAcademicOfferingUseCase deactivateAcademicOfferingUseCase,
        ActivateAcademicOfferingUseCase activateAcademicOfferingUseCase,
        GetAcademicOfferingByIdUseCase getAcademicOfferingByIdUseCase,
        ITenantContext tenantContext)
    {
        _createAcademicOfferingUseCase = createAcademicOfferingUseCase;
        _updateAcademicOfferingUseCase = updateAcademicOfferingUseCase;
        _deactivateAcademicOfferingUseCase = deactivateAcademicOfferingUseCase;
        _activateAcademicOfferingUseCase  = activateAcademicOfferingUseCase;
        _getAcademicOfferingByIdUseCase = getAcademicOfferingByIdUseCase;
        _tenantContext = tenantContext;

    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAcademicOfferingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateAcademicOfferingResponse>> CreateAsync([FromBody] CreateAcademicOfferingRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createAcademicOfferingUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicOfferingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicOfferingResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateAcademicOfferingRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
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
        await _deactivateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
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
        await _activateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AcademicOfferingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicOfferingDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var response = await _getAcademicOfferingByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);
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