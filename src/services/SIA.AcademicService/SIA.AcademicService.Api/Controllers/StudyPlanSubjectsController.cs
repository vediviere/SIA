using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

namespace SIA.AcademicService.Api.Controllers;


[ApiController]
[Route("api/study-plan-subjects")]
public sealed class StudyPlanSubjectsController : ControllerBase
{
    private readonly CreateStudyPlanSubjectUseCase _createStudyPlanSubjectUseCase;
    private readonly UpdateStudyPlanSubjectUseCase _updateStudyPlanSubjectUseCase;
    private readonly DeleteStudyPlanSubjectUseCase _deleteStudyPlanSubjectUseCase;
    private readonly RestoreStudyPlanSubjectUseCase _restoreStudyPlanSubjectUseCase;
    private readonly IStudyPlanQueries _studyPlanQueries;

    public StudyPlanSubjectsController(
        CreateStudyPlanSubjectUseCase createStudyPlanSubjectUseCase,
        UpdateStudyPlanSubjectUseCase updateStudyPlanSubjectUseCase,
        DeleteStudyPlanSubjectUseCase deleteStudyPlanSubjectUseCase,
        RestoreStudyPlanSubjectUseCase restoreStudyPlanSubjectUseCase,
        IStudyPlanQueries studyPlanQueries)
    {
        _createStudyPlanSubjectUseCase = createStudyPlanSubjectUseCase;
        _updateStudyPlanSubjectUseCase = updateStudyPlanSubjectUseCase;
        _deleteStudyPlanSubjectUseCase = deleteStudyPlanSubjectUseCase;
        _restoreStudyPlanSubjectUseCase = restoreStudyPlanSubjectUseCase;
        _studyPlanQueries = studyPlanQueries;
    }

    [HttpGet("{tenantId:guid}/study-plan/{studyPlanId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<StudyPlanSubjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StudyPlanSubjectDto>>> GetSubjectsByStudyPlanAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid studyPlanId,
        CancellationToken cancellationToken)
    {
        var subjects = await _studyPlanQueries.GetSubjectsByStudyPlanAsync(tenantId, studyPlanId, cancellationToken);
        return Ok(subjects);
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

        try
        {
            var response = await _createStudyPlanSubjectUseCase.ExecuteAsync(request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message,
                correlationId
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message,
                correlationId
            });
        }
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateStudyPlanSubjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateStudyPlanSubjectResponse>> UpdateAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        [FromBody] UpdateStudyPlanSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        try
        {
            var response = await _updateStudyPlanSubjectUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
            return Ok(response);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("asignada") || exception.Message.Contains("existe"))
        {
            return Conflict(new { message = exception.Message, correlationId });
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message, correlationId });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message, correlationId });
        }
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            // Mapeamos los datos de la ruta al Request creado en el paso anterior
            var request = new DeleteStudyPlanSubjectRequest
            {
                TenantId = tenantId,
                Id = id
            };

            await _deleteStudyPlanSubjectUseCase.ExecuteAsync(request, correlationId, cancellationToken);
            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new
            {
                message = exception.Message,
                correlationId
            });
        }
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        try
        {
            var request = new RestoreStudyPlanSubjectRequest
            {
                TenantId = tenantId,
                Id = id
            };

            await _restoreStudyPlanSubjectUseCase.ExecuteAsync(request, correlationId, cancellationToken);
            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message, correlationId });
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
