using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.DivisionHeads;
using SIA.AcademicStaffService.Application.Interfaces;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.DivisionHeads;
using SIA.AcademicStaffService.Contracts.Requests.DivisionHeads;
using SIA.AcademicStaffService.Contracts.Responses.DivisionHeads;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/division-heads")]
public sealed class DivisionHeadsController : ControllerBase
{
    private readonly CreateDivisionHeadUseCase _createDivisionHeadUseCase;
    private readonly ActivateDivisionHeadUseCase _activateDivisionHeadUseCase;
    private readonly DeactivateDivisionHeadUseCase _deactivateDivisionHeadUseCase;
    private readonly IDivisionHeadQueries _divisionHeadQueries;
    private readonly ITenantContext _tenantContext;

    public DivisionHeadsController(
        CreateDivisionHeadUseCase createDivisionHeadUseCase,
        ActivateDivisionHeadUseCase activateDivisionHeadUseCase,
        DeactivateDivisionHeadUseCase deactivateDivisionHeadUseCase,
        IDivisionHeadQueries divisionHeadQueries,
        ITenantContext tenantContext)
    {
        _createDivisionHeadUseCase = createDivisionHeadUseCase;
        _activateDivisionHeadUseCase = activateDivisionHeadUseCase;
        _deactivateDivisionHeadUseCase = deactivateDivisionHeadUseCase;
        _divisionHeadQueries = divisionHeadQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DivisionHead>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<DivisionHead>>> SearchAsync([FromQuery] DivisionHeadFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new DivisionHeadFilter
        {
            TenantId = _tenantContext.TenantId,
            ProgramId = filter.ProgramId,
            PersonId = filter.PersonId,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var divisionHeads = await _divisionHeadQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(divisionHeads);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DivisionHead), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DivisionHead>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var divisionHead = await _divisionHeadQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (divisionHead == null)
        {
            return NotFound(new { message = $"No se encontró el responsable de división con Id {id}." });
        }

        return Ok(divisionHead);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateDivisionHeadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateDivisionHeadResponse>> CreateAsync([FromBody] CreateDivisionHeadRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createDivisionHeadUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

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

        await _activateDivisionHeadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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

        await _deactivateDivisionHeadUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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