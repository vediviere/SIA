using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.Requests;
using SIA.AcademicService.Contracts.Responses;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/EducationalPrograms")]

public sealed class EducationalProgramsController : ControllerBase
{
    private readonly CreateEducationalProgramsUseCase _createEducationalProgramsUseCase;

    public EducationalProgramsController(CreateEducationalProgramsUseCase createEducationalProgramsUseCase)
    {
        _createEducationalProgramsUseCase = createEducationalProgramsUseCase;
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
    