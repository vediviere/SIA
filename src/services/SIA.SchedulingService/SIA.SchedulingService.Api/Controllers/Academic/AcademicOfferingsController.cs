using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.AcademicLoad;
using SIA.SchedulingService.Application.DTOs.AcademicOffering;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Contracts.Responses;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.AcademicOffering;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/AcademicOffering")]
public class AcademicOfferingsController : ControllerBase
{
    private readonly CreateAcademicOfferingUseCase _createAcademicOfferingUseCase;
    private readonly UpdateAcademicOfferingUseCase _updateAcademicOfferingUseCase;
    private readonly DeactivateAcademicOfferingUseCase _deactivateAcademicOfferingUseCase;
    private readonly ActivateAcademicOfferingUseCase _activateAcademicOfferingUseCase;
    private readonly GetAcademicOfferingByIdUseCase _getAcademicOfferingByIdUseCase;

    public AcademicOfferingsController(
        CreateAcademicOfferingUseCase createAcademicOfferingUseCase, 
        UpdateAcademicOfferingUseCase updateAcademicOfferingUseCase,
        DeactivateAcademicOfferingUseCase deactivateAcademicOfferingUseCase,
        ActivateAcademicOfferingUseCase activateAcademicOfferingUseCase,
        GetAcademicOfferingByIdUseCase getAcademicOfferingByIdUseCase)
    {
        _createAcademicOfferingUseCase = createAcademicOfferingUseCase;
        _updateAcademicOfferingUseCase = updateAcademicOfferingUseCase;
        _deactivateAcademicOfferingUseCase = deactivateAcademicOfferingUseCase;
        _activateAcademicOfferingUseCase  = activateAcademicOfferingUseCase;
        _getAcademicOfferingByIdUseCase = getAcademicOfferingByIdUseCase;
        
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAcademicOfferingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateAcademicOfferingResponse>> CreateAsync([FromBody] CreateAcademicOfferingRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createAcademicOfferingUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicOfferingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicOfferingResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateAcademicOfferingRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _activateAcademicOfferingUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(AcademicOfferingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicOfferingDto>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
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