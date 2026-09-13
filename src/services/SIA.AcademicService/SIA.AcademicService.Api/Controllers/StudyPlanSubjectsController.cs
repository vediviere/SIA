
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Api.Extensions;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Application.DTOs.StudyPlanSubjects;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/study-plan-subjects")]
[Authorize]
public sealed class StudyPlanSubjectsController : ControllerBase
{
    private readonly CreateStudyPlanSubjectUseCase _createUseCase;
    private readonly UpdateStudyPlanSubjectUseCase _updateUseCase;
    private readonly SoftDeleteStudyPlanSubjectUseCase _softDeleteUseCase;
    private readonly RestoreStudyPlanSubjectUseCase _restoreUseCase;
    private readonly IStudyPlanSubjectQueries _queries;
    private readonly IStudyPlanQueries _studyPlanQueries;

    public StudyPlanSubjectsController(
        CreateStudyPlanSubjectUseCase createUseCase,
        UpdateStudyPlanSubjectUseCase updateUseCase,
        SoftDeleteStudyPlanSubjectUseCase softDeleteUseCase,
        RestoreStudyPlanSubjectUseCase restoreUseCase,
        IStudyPlanSubjectQueries queries,
        IStudyPlanQueries studyPlanQueries)
    {
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _softDeleteUseCase = softDeleteUseCase;
        _restoreUseCase = restoreUseCase;
        _queries = queries;
        _studyPlanQueries = studyPlanQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<StudyPlanSubject>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StudyPlanSubject>>> SearchAsync(
        [FromQuery] StudyPlanSubjectFilter filter,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();
        var results = await _queries.SearchAsync(tenantId, filter, cancellationToken);
        return Ok(results);
    }

    [HttpGet("study-plan/{studyPlanId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<StudyPlanSubjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StudyPlanSubjectDto>>> GetSubjectsByStudyPlanAsync(
        [FromRoute] Guid studyPlanId,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();
        var subjects = await _studyPlanQueries.GetSubjectsByStudyPlanAsync(tenantId, studyPlanId, cancellationToken);
        return Ok(subjects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudyPlanSubject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudyPlanSubject>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();
        var result = await _queries.GetByIdAsync(tenantId, id, cancellationToken);

        if (result == null)
        {
            throw new StudyPlanSubjectNotFoundException(id);
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateStudyPlanSubjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateStudyPlanSubjectResponse>> CreateAsync(
        [FromBody] CreateStudyPlanSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();
        var response = await _createUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateStudyPlanSubjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateStudyPlanSubjectResponse>> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateStudyPlanSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();

        var response = await _updateUseCase.ExecuteAsync(
            tenantId,
            id,
            request,
            correlationId,
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();

        await _softDeleteUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();

        await _restoreUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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