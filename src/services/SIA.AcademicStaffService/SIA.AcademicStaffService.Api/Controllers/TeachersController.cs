using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.Professors;
using SIA.AcademicStaffService.Application.Interfaces;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Contracts.Requests.Professors;
using SIA.AcademicStaffService.Contracts.Responses.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/teachers")]
public sealed class TeachersController : ControllerBase
{
    private readonly CreateTeacherUseCase _createProfessorUseCase;
    private readonly UpdateTeacherUseCase _updateProfessorUseCase;
    private readonly ActivateTeacherUseCase _activateProfessorUseCase;
    private readonly DeactivateTeacherUseCase _deactivateProfessorUseCase;
    private readonly ITeacherQueries _professorQueries;
    private readonly ITenantContext _tenantContext;

    public TeachersController(
        CreateTeacherUseCase createProfessorUseCase,
        UpdateTeacherUseCase updateProfessorUseCase,
        ActivateTeacherUseCase activateProfessorUseCase,
        DeactivateTeacherUseCase deactivateProfessorUseCase,
        ITeacherQueries professorQueries,
        ITenantContext tenantContext)
    {
        _createProfessorUseCase = createProfessorUseCase;
        _updateProfessorUseCase = updateProfessorUseCase;
        _activateProfessorUseCase = activateProfessorUseCase;
        _deactivateProfessorUseCase = deactivateProfessorUseCase;
        _professorQueries = professorQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Teacher>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Teacher>>> SearchAsync([FromQuery] TeacherFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new TeacherFilter
        {
            TenantId = _tenantContext.TenantId,
            PersonId = filter.PersonId,
            ContractType = filter.ContractType,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var professors = await _professorQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(professors);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Teacher), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Teacher>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var professor = await _professorQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (professor == null)
        {
            return NotFound(new { message = $"No se encontró el profesor con Id {id}." });
        }

        return Ok(professor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTeacherResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateTeacherResponse>> CreateAsync([FromBody] CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createProfessorUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateTeacherResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateProfessorUseCase.ExecuteAsync(
            tenantId,
            id,
            request,
            correlationId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        var tenantId = _tenantContext.TenantId;

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _activateProfessorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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

        await _deactivateProfessorUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpGet("candidates")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CandidateTeacherResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CandidateTeacherResponse>>> GetCandidatesAsync(CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var teachers = await _professorQueries.GetCandidatesAsync(
            new CandidateTeacherFilter { TenantId = tenantId },
            cancellationToken);

        var response = teachers.Select(teacher => new CandidateTeacherResponse
        {
            TeacherId = teacher.Id,
            ProfessionalProfile = teacher.ProfessionalProfile,
            ProgramId = teacher.ProgramId,
            ContractHours = teacher.ContractHours,
            Status = teacher.Status
        }).ToList();

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