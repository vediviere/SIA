using Microsoft.AspNetCore.Mvc;
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
    private readonly IEducationalProgramsQueries _queries;
    private readonly UpdateEducationalProgramsUseCase _updateUseCase;
    private readonly DeactivateEducationalProgramsUseCase _deactivateUseCase;
    private readonly RestoreEducationalProgramsUseCase _restoreUseCase;

    public EducationalProgramsController(
        CreateEducationalProgramsUseCase createEducationalProgramsUseCase, 
        IEducationalProgramsQueries queries,
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

        try
        {
            var response = await _createEducationalProgramsUseCase.ExecuteAsync(request, correlationId, cancellationToken);

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message, correlationId });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message, correlationId });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<EducationalProgram>>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await _queries.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EducationalProgram>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateEducationalProgramsResponse>> UpdateAsync(Guid id, [FromBody] UpdateEducationalProgramsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _updateUseCase.ExecuteAsync(id, request, cancellationToken));
        }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _deactivateUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> RestoreAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _restoreUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
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
    