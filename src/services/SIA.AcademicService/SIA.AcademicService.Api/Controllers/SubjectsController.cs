using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.Requests;
using SIA.AcademicService.Contracts.Responses;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/subjects")]
public sealed class SubjectsController : ControllerBase
{
  private readonly CreateSubjectUseCase _createSubjectUseCase;

  public SubjectsController(
      CreateSubjectUseCase createSubjectUseCase)
  {
    _createSubjectUseCase = createSubjectUseCase;
  }

  [HttpPost]
  [ProducesResponseType(
      typeof(CreateSubjectResponse),
      StatusCodes.Status201Created)]
  [ProducesResponseType(
      StatusCodes.Status400BadRequest)]
  [ProducesResponseType(
      StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateSubjectResponse>> CreateAsync([FromBody] CreateSubjectRequest request,
      CancellationToken cancellationToken)
  {
    var correlationId = ResolveCorrelationId();

    try
    {
      var response =
          await _createSubjectUseCase.ExecuteAsync(request, correlationId, cancellationToken);

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
