using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.Professors;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Contracts.Responses.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[ApiController]
[Route("api/professors")]
public sealed class ProfessorsController : ControllerBase
{
    private readonly CreateProfessorUseCase _createProfessorUseCase;
    private readonly UpdateProfessorUseCase _updateProfessorUseCase;
    private readonly ActivateProfessorUseCase _activateProfessorUseCase;
    private readonly DeactivateProfessorUseCase _deactivateProfessorUseCase;
    private readonly IProfessorQueries _professorQueries;

    public ProfessorsController(
        CreateProfessorUseCase createProfessorUseCase,
        UpdateProfessorUseCase updateProfessorUseCase,
        ActivateProfessorUseCase activateProfessorUseCase,
        DeactivateProfessorUseCase deactivateProfessorUseCase,
        IProfessorQueries professorQueries)
    {
        _createProfessorUseCase = createProfessorUseCase;
        _updateProfessorUseCase = updateProfessorUseCase;
        _activateProfessorUseCase = activateProfessorUseCase;
        _deactivateProfessorUseCase = deactivateProfessorUseCase;
        _professorQueries = professorQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Professor>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Professor>>> SearchAsync([FromQuery] ProfessorFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new ProfessorFilter
        {
            TenantId = filter.TenantId,
            PersonId = filter.PersonId,
            ContractType = filter.ContractType,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var professors = await _professorQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(professors);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(Professor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Professor>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var professor = await _professorQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (professor == null)
        {
            return NotFound(new { message = $"No se encontró el profesor con Id {id}." });
        }

        return Ok(professor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProfessorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateProfessorResponse>> CreateAsync([FromBody] CreateProfessorRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append(
            "X-Correlation-Id",
            correlationId.ToString());

        var response = await _createProfessorUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateProfessorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateProfessorResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateProfessorRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateProfessorUseCase.ExecuteAsync(
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

        await _activateProfessorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _deactivateProfessorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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