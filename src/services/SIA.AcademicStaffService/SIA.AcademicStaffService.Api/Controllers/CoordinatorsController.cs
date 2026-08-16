using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.Coordinators;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Contracts.Requests.Coordinators;
using SIA.AcademicStaffService.Contracts.Responses.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[ApiController]
[Route("api/coordinators")]
public sealed class CoordinatorsController : ControllerBase
{
    private readonly CreateCoordinatorUseCase _createCoordinatorUseCase;
    private readonly ActivateCoordinatorUseCase _activateCoordinatorUseCase;
    private readonly DeactivateCoordinatorUseCase _deactivateCoordinatorUseCase;
    private readonly ICoordinatorQueries _coordinatorQueries;

    public CoordinatorsController(
        CreateCoordinatorUseCase createCoordinatorUseCase,
        ActivateCoordinatorUseCase activateCoordinatorUseCase,
        DeactivateCoordinatorUseCase deactivateCoordinatorUseCase,
        ICoordinatorQueries coordinatorQueries)
    {
        _createCoordinatorUseCase = createCoordinatorUseCase;
        _activateCoordinatorUseCase = activateCoordinatorUseCase;
        _deactivateCoordinatorUseCase = deactivateCoordinatorUseCase;
        _coordinatorQueries = coordinatorQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Coordinator>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Coordinator>>> SearchAsync([FromQuery] CoordinatorFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new CoordinatorFilter
        {
            TenantId = filter.TenantId,
            PersonId = filter.PersonId,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var coordinators = await _coordinatorQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(coordinators);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(Coordinator), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Coordinator>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var coordinator = await _coordinatorQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (coordinator == null)
        {
            return NotFound(new { message = $"No se encontró el coordinador con Id {id}." });
        }

        return Ok(coordinator);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCoordinatorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCoordinatorResponse>> CreateAsync([FromBody] CreateCoordinatorRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append(
            "X-Correlation-Id",
            correlationId.ToString());

        var response = await _createCoordinatorUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _activateCoordinatorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{tenantId:guid}/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

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