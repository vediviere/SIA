using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.DTOs.Subjects;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Contracts.Responses.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/subjects")]
public sealed class SubjectsController : ControllerBase
{
  private readonly CreateSubjectUseCase _createSubjectUseCase;
  private readonly UpdateSubjectUseCase _updateSubjectUseCase;
  private readonly SoftDeleteSubjectUseCase _softDeleteSubjectUseCase;
  private readonly RestoreSubjectUseCase _restoreSubjectUseCase;
  private readonly ISubjectQueries _subjectQueries;

  public SubjectsController(CreateSubjectUseCase createSubjectUseCase, UpdateSubjectUseCase updateSubjectUseCase,  SoftDeleteSubjectUseCase softDeleteSubjectUseCase, RestoreSubjectUseCase restoreSubjectUseCase, ISubjectQueries subjectQueries)
  {
    _createSubjectUseCase = createSubjectUseCase;
    _updateSubjectUseCase = updateSubjectUseCase;
    _softDeleteSubjectUseCase = softDeleteSubjectUseCase;
    _restoreSubjectUseCase = restoreSubjectUseCase;
    _subjectQueries = subjectQueries;
  }


  [HttpGet("Filter")]
  [ProducesResponseType(typeof(IReadOnlyCollection<Subject>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IReadOnlyCollection<Subject>>> SearchAsync([FromQuery] SubjectFilter filter,  CancellationToken cancellationToken)
  {
    var secureFilter = new SubjectFilter
    {
      TenantId = filter.TenantId,
      Code = filter.Code,
      Name = filter.Name,
      Semester = filter.Semester,
      Status = filter.Status,
      Page = filter.Page,
      PageSize = filter.PageSize
    };

    var subjects = await _subjectQueries.SearchAsync(secureFilter, cancellationToken);
    return Ok(subjects);
  }


  [HttpGet("{tenantId:guid}/{id:guid}")]
  [ProducesResponseType(typeof(Subject), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<Subject>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var subject = await _subjectQueries.GetByIdAsync(tenantId, id, cancellationToken);

    if (subject == null)
    {
      return NotFound(new { message = $"No se encontró la materia con Id {id}." });
    }

    return Ok(subject);
  }


  [HttpPost]
  [ProducesResponseType(typeof(CreateSubjectResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateSubjectResponse>> CreateAsync([FromBody] CreateSubjectRequest request,  CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();

    Response.Headers.Append(
        "X-Correlation-Id",
        correlationId.ToString());

    var response = await _createSubjectUseCase.ExecuteAsync(request, correlationId, cancellationToken);

    return StatusCode(StatusCodes.Status201Created, response);
  }


  [HttpPut("{tenantId:guid}/{id:guid}")]
  [ProducesResponseType(typeof(UpdateSubjectResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<UpdateSubjectResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id,  [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();

    Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

    var response = await _updateSubjectUseCase.ExecuteAsync(
        tenantId,
        id,
        request,
        correlationId,
        cancellationToken);

    return Ok(response);
  }


  [HttpDelete("{tenantId:guid}/{id:guid}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> SoftDeleteAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();

    Response.Headers.Append(
        "X-Correlation-Id",
        correlationId.ToString());

    await _softDeleteSubjectUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

    return NoContent();
  }


  [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();

    Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

    await _restoreSubjectUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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
