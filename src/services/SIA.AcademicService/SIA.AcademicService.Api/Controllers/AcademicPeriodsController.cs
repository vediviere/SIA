using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;

using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/academic-periods")]
public sealed class AcademicPeriodsController : ControllerBase
{
    private readonly CreateAcademicPeriodsUseCase _createAcademicPeriodsUseCase;
    private readonly UpdateAcademicPeriodUseCase _updateAcademicPeriodUseCase;
    private readonly PatchAcademicPeriodUseCase _patchAcademicPeriodUseCase;
    private readonly DeactivateAcademicPeriodUseCase _deactivateAcademicPeriodUseCase;
    private readonly ActivateAcademicPeriodUseCase _activateAcademicPeriodUseCase;
    private readonly SearchAcademicPeriodsUseCase _searchAcademicPeriodsUseCase;
    private readonly GetAcademicPeriodByIdUseCase _getAcademicPeriodByIdUseCase;

    public AcademicPeriodsController(
        CreateAcademicPeriodsUseCase createAcademicPeriodsUseCase,
        UpdateAcademicPeriodUseCase updateAcademicPeriodUseCase,
        PatchAcademicPeriodUseCase patchAcademicPeriodUseCase,
        DeactivateAcademicPeriodUseCase deactivateAcademicPeriodUseCase,
        ActivateAcademicPeriodUseCase activateAcademicPeriodUseCase,
        SearchAcademicPeriodsUseCase SearchAcademicPeriodsUseCase,
        GetAcademicPeriodByIdUseCase getAcademicPeriodByIdUseCase)
    {
        _createAcademicPeriodsUseCase = createAcademicPeriodsUseCase;
        _updateAcademicPeriodUseCase = updateAcademicPeriodUseCase;
        _patchAcademicPeriodUseCase = patchAcademicPeriodUseCase;
        _deactivateAcademicPeriodUseCase = deactivateAcademicPeriodUseCase;
        _activateAcademicPeriodUseCase = activateAcademicPeriodUseCase;
        _searchAcademicPeriodsUseCase = SearchAcademicPeriodsUseCase;
        _getAcademicPeriodByIdUseCase = getAcademicPeriodByIdUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAcademicPeriodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateAcademicPeriodResponse>> CreateAsync([FromBody] CreateAcademicPeriodRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createAcademicPeriodsUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicPeriodResponse>> UpdateAsync([FromRoute] Guid id, [FromRoute] Guid tenantId, [FromBody] UpdateAcademicPeriodRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateAcademicPeriodUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(PatchAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PatchAcademicPeriodResponse>> PatchAsync([FromRoute] Guid id,[FromRoute] Guid tenantId, [FromBody] PatchAcademicPeriodRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _patchAcademicPeriodUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(DeactivateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeactivateAcademicPeriodResponse>> DeactivateAsync([FromRoute] Guid id,[FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _deactivateAcademicPeriodUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(typeof(ActivateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivateAcademicPeriodResponse>> ActivateAsync([FromRoute] Guid id,[FromRoute] Guid tenantId,CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var respuesta = await _activateAcademicPeriodUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return Ok(respuesta);
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AcademicPeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AcademicPeriodDto>>> SearchAsync([FromQuery] AcademicPeriodFilter filter,CancellationToken cancellationToken)
    {
        var response = await _searchAcademicPeriodsUseCase.ExecuteAsync(filter, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(AcademicPeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicPeriodDto>> GetByIdAsync([FromRoute] Guid id,[FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var response = await _getAcademicPeriodByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);

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