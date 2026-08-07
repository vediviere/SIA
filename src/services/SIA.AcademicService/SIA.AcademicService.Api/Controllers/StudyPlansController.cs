using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.StudyPlans;
using SIA.AcademicService.Contracts.Requests.StudyPlans;
using SIA.AcademicService.Contracts.Responses.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/study-plans")]
public sealed class StudyPlansController : ControllerBase
{
    private readonly CreateStudyPlanUseCase _createUseCase;
    private readonly UpdateStudyPlanUseCase _updateUseCase;
    private readonly DeactivateStudyPlanUseCase _deactivateUseCase;
    private readonly RestoreStudyPlanUseCase _restoreUseCase;
    private readonly IStudyPlanQueries _queries;

    public StudyPlansController(
        CreateStudyPlanUseCase createUseCase,
        UpdateStudyPlanUseCase updateUseCase,
        DeactivateStudyPlanUseCase deactivateUseCase,
        RestoreStudyPlanUseCase restoreUseCase,
        IStudyPlanQueries queries)
    {
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deactivateUseCase = deactivateUseCase;
        _restoreUseCase = restoreUseCase;
        _queries = queries;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateStudyPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateStudyPlanResponse>> CreateAsync([FromBody] CreateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<StudyPlan>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StudyPlan>>> SearchAsync(
        [FromQuery] StudyPlanFilter filter,
        CancellationToken cancellationToken)
    {
        var secureFilter = new StudyPlanFilter
        {
            TenantId = filter.TenantId,
            EducationalProgramId = filter.EducationalProgramId,
            Code = filter.Code,
            Name = filter.Name,
            Version = filter.Version,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var results = await _queries.SearchAsync(secureFilter, cancellationToken);
        return Ok(results);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(StudyPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudyPlan>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(tenantId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateStudyPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateStudyPlanResponse>> UpdateAsync(
        [FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
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