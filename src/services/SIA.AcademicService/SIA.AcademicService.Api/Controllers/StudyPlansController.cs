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
    public async Task<ActionResult<CreateStudyPlanResponse>> CreateAsync([FromBody] CreateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        try
        {
            var response = await _createUseCase.ExecuteAsync(request, correlationId, cancellationToken);
            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message, correlationId }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message, correlationId }); }
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] StudyPlanFilter filter, CancellationToken cancellationToken)
    {
        var results = await _queries.SearchAsync(filter, cancellationToken);
        return Ok(results);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    public async Task<ActionResult<StudyPlan>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(tenantId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateStudyPlanResponse>> UpdateAsync(Guid id, [FromBody] UpdateStudyPlanRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _updateUseCase.ExecuteAsync(id, request, cancellationToken)); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        try { await _deactivateUseCase.ExecuteAsync(id, cancellationToken); return NoContent(); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> RestoreAsync(Guid id, CancellationToken cancellationToken)
    {
        try { await _restoreUseCase.ExecuteAsync(id, cancellationToken); return NoContent(); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
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