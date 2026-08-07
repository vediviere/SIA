using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.DTOs.EducationalProgram;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
using SIA.AcademicService.Contracts.Responses.EducationalProgramsResponse;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/EducationalPrograms")]

public sealed class EducationalProgramsController : ControllerBase
{
    private readonly CreateEducationalProgramsUseCase _createEducationalProgramsUseCase;
    private readonly IEducationalProgramQueries _queries;
    private readonly UpdateEducationalProgramsUseCase _updateUseCase;
    private readonly DeactivateEducationalProgramsUseCase _deactivateUseCase;
    private readonly RestoreEducationalProgramsUseCase _restoreUseCase;

    public EducationalProgramsController(
        CreateEducationalProgramsUseCase createEducationalProgramsUseCase, 
        IEducationalProgramQueries queries,
        UpdateEducationalProgramsUseCase updateUseCase,
        DeactivateEducationalProgramsUseCase deactivateUseCase,
        RestoreEducationalProgramsUseCase restoreUseCase)
    {
        _createEducationalProgramsUseCase = createEducationalProgramsUseCase;
        _queries = queries;
        _updateUseCase = updateUseCase;
        _deactivateUseCase = deactivateUseCase;
        _restoreUseCase = restoreUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateEducationalProgramsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateEducationalProgramsResponse>> CreateAsync([FromBody] CreateEducationalProgramsRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _createEducationalProgramsUseCase.ExecuteAsync(request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EducationalProgram>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EducationalProgram>>> SearchAsync(
        [FromQuery] EducationalProgramFilter filter,
        CancellationToken cancellationToken)
    {
        var secureFilter = new EducationalProgramFilter
        {
            TenantId = filter.TenantId,
            Code = filter.Code,
            Name = filter.Name,
            Level = filter.Level,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var programs = await _queries.SearchAsync(secureFilter, cancellationToken);
        return Ok(programs);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(EducationalProgram), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EducationalProgram>> GetByIdAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(tenantId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateEducationalProgramsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateEducationalProgramsResponse>> UpdateAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        [FromBody] UpdateEducationalProgramsRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        var response = await _updateUseCase.ExecuteAsync(tenantId, id, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());
        await _deactivateUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);
        return NoContent();
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
    