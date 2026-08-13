using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.DivisionManagers;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;
using SIA.AcademicStaffService.Contracts.Responses.DivisionManagers;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[ApiController]
[Route("api/division-heads")]
public sealed class DivisionHeadsController : ControllerBase
{
    private readonly CreateDivisionHeadUseCase _createDivisionManagerUseCase;
    private readonly UpdateDivisionHeadUseCase _updateDivisionManagerUseCase;
    private readonly ActivateDivisionHeadUseCase _activateDivisionManagerUseCase;
    private readonly DeactivateDivisionHeadUseCase _deactivateDivisionManagerUseCase;
    private readonly IDivisionHeadQueries _divisionManagerQueries;

    public DivisionHeadsController(
        CreateDivisionHeadUseCase createDivisionManagerUseCase,
        UpdateDivisionHeadUseCase updateDivisionManagerUseCase,
        ActivateDivisionHeadUseCase activateDivisionManagerUseCase,
        DeactivateDivisionHeadUseCase deactivateDivisionManagerUseCase,
        IDivisionHeadQueries divisionManagerQueries)
    {
        _createDivisionManagerUseCase = createDivisionManagerUseCase;
        _updateDivisionManagerUseCase = updateDivisionManagerUseCase;
        _activateDivisionManagerUseCase = activateDivisionManagerUseCase;
        _deactivateDivisionManagerUseCase = deactivateDivisionManagerUseCase;
        _divisionManagerQueries = divisionManagerQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DivisionHead>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<DivisionHead>>> SearchAsync([FromQuery] DivisionHeadFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new DivisionHeadFilter
        {
            TenantId = filter.TenantId,
            ProgramId = filter.ProgramId,
            PersonId = filter.PersonId,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var divisionManagers = await _divisionManagerQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(divisionManagers);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(DivisionHead), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DivisionHead>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var divisionManager = await _divisionManagerQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (divisionManager == null)
        {
            return NotFound(new { message = $"No se encontró el responsable de división con Id {id}." });
        }

        return Ok(divisionManager);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateDivisionHeadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateDivisionHeadResponse>> CreateAsync([FromBody] CreateDivisionHeadRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append(
            "X-Correlation-Id",
            correlationId.ToString());

        var response = await _createDivisionManagerUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateDivisionHeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateDivisionHeadResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateDivisionHeadRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateDivisionManagerUseCase.ExecuteAsync(
            tenantId,
            id,
            request,
            correlationId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _activateDivisionManagerUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{tenantId:guid}/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _deactivateDivisionManagerUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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