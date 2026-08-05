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

        try
        {
            var response = await _createAcademicPeriodsUseCase.ExecuteAsync(request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message, correlationId });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message, correlationId });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateAcademicPeriodResponse>> UpdateAsync(Guid id, [FromBody] UpdateAcademicPeriodRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            var response = await _updateAcademicPeriodUseCase.ExecuteAsync(id, request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return Ok(response);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("No existe"))
        {
            return NotFound(new { message = exception.Message, correlationId });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message, correlationId });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message, correlationId });
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(PatchAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PatchAcademicPeriodResponse>> PatchAsync(Guid id, [FromBody] PatchAcademicPeriodRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            var response = await _patchAcademicPeriodUseCase.ExecuteAsync(id, request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return Ok(response);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("No existe"))
        {
            return NotFound(new { message = exception.Message, correlationId });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message, correlationId });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message, correlationId });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DeactivateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeactivateAcademicPeriodResponse>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            var response = await _deactivateAcademicPeriodUseCase.ExecuteAsync(id, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message, correlationId });
        }
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(typeof(ActivateAcademicPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivateAcademicPeriodResponse>> ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            var response = await _activateAcademicPeriodUseCase.ExecuteAsync(id, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message, correlationId });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AcademicPeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AcademicPeriodDto>>> SearchAsync(
    [FromQuery] Guid tenantId,
    [FromQuery] string? code,
    [FromQuery] string? name,
    [FromQuery] bool? status,
    [FromQuery] int page,
    [FromQuery] int pageSize,
    CancellationToken cancellationToken)
    {
        var filter = new AcademicPeriodFilter
        {
            TenantId = tenantId,
            Code = code,
            Name = name,
            Status = status,
            Page = page <= 0 ? 1 : page,
            PageSize = pageSize <= 0 ? 10 : pageSize
        };

        var response = await _searchAcademicPeriodsUseCase.ExecuteAsync(filter, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AcademicPeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicPeriodDto>> GetByIdAsync(Guid id, [FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getAcademicPeriodByIdUseCase.ExecuteAsync(tenantId, id, cancellationToken);

            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
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