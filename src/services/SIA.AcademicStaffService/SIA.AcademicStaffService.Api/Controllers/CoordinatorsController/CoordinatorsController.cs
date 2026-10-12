using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.Coordinators;
using SIA.AcademicStaffService.Application.Interfaces;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Contracts.Requests.Coordinators;
using SIA.AcademicStaffService.Contracts.Responses.Coordinators;

namespace SIA.AcademicStaffService.Api.Controllers.CoordinatorsController;

[Authorize]
[ApiController]
[Route("api/coordinators")]
public sealed class CoordinatorsController : ControllerBase
{
    private readonly CreateCoordinatorUseCase _createCoordinatorUseCase;
    private readonly ActivateCoordinatorUseCase _activateCoordinatorUseCase;
    private readonly DeactivateCoordinatorUseCase _deactivateCoordinatorUseCase;
    private readonly ICoordinatorQueries _coordinatorQueries;
    private readonly ITenantContext _tenantContext;

    public CoordinatorsController(
        CreateCoordinatorUseCase createCoordinatorUseCase,
        ActivateCoordinatorUseCase activateCoordinatorUseCase,
        DeactivateCoordinatorUseCase deactivateCoordinatorUseCase,
        ICoordinatorQueries coordinatorQueries,
        ITenantContext tenantContext)
    {
        _createCoordinatorUseCase = createCoordinatorUseCase;
        _activateCoordinatorUseCase = activateCoordinatorUseCase;
        _deactivateCoordinatorUseCase = deactivateCoordinatorUseCase;
        _coordinatorQueries = coordinatorQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CoordinatorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CoordinatorResponse>>> SearchAsync([FromQuery] CoordinatorFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new CoordinatorFilter
        {
            TenantId = _tenantContext.TenantId,
            PersonId = filter.PersonId,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var coordinators = await _coordinatorQueries.SearchAsync(secureFilter, cancellationToken);

        var response = coordinators.Select(c => new CoordinatorResponse
        {
            Id = c.Id,
            TenantId = c.TenantId,
            PersonId = c.PersonId,
            Status = c.Status,
            CreatedAtUtc = c.CreatedAtUtc,
            UpdatedAtUtc = c.UpdatedAtUtc
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CoordinatorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoordinatorResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var c = await _coordinatorQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (c == null)
        {
            return NotFound(new { message = $"No se encontró el coordinador con Id {id}." });
        }

        var response = new CoordinatorResponse
        {
            Id = c.Id,
            TenantId = c.TenantId,
            PersonId = c.PersonId,
            Status = c.Status,
            CreatedAtUtc = c.CreatedAtUtc,
            UpdatedAtUtc = c.UpdatedAtUtc
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCoordinatorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCoordinatorResponse>> CreateAsync([FromBody] CreateCoordinatorRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createCoordinatorUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _activateCoordinatorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _deactivateCoordinatorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
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