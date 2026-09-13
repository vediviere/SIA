using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Api.Extensions;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.DTOs.Subjects;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Contracts.Responses.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/subjects")]
[Authorize]
public sealed class SubjectsController : ControllerBase
{
    private readonly CreateSubjectUseCase _createSubjectUseCase;
    private readonly UpdateSubjectUseCase _updateSubjectUseCase;
    private readonly SoftDeleteSubjectUseCase _softDeleteSubjectUseCase;
    private readonly RestoreSubjectUseCase _restoreSubjectUseCase;
    private readonly ISubjectQueries _subjectQueries;

    public SubjectsController(CreateSubjectUseCase createSubjectUseCase, UpdateSubjectUseCase updateSubjectUseCase, SoftDeleteSubjectUseCase softDeleteSubjectUseCase, RestoreSubjectUseCase restoreSubjectUseCase, ISubjectQueries subjectQueries)
    {
        _createSubjectUseCase = createSubjectUseCase;
        _updateSubjectUseCase = updateSubjectUseCase;
        _softDeleteSubjectUseCase = softDeleteSubjectUseCase;
        _restoreSubjectUseCase = restoreSubjectUseCase;
        _subjectQueries = subjectQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Subject>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Subject>>> SearchAsync([FromQuery] SubjectFilter filter, CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();
        var subjects = await _subjectQueries.SearchAsync(tenantId, filter, cancellationToken);
        return Ok(subjects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Subject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Subject>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var subject = await _subjectQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (subject == null)
        {
            throw new SubjectNotFoundException(id);
        }

        return Ok(subject);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSubjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateSubjectResponse>> CreateAsync([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();
        var response = await _createSubjectUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateSubjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateSubjectResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();

        var response = await _updateSubjectUseCase.ExecuteAsync(
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
    public async Task<IActionResult> SoftDeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append(
            "X-Correlation-Id",
            correlationId.ToString());

        var tenantId = User.GetTenantId();

        await _softDeleteSubjectUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var tenantId = User.GetTenantId();

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