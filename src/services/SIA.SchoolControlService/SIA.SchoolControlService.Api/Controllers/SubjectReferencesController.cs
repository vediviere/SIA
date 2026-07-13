using Microsoft.AspNetCore.Mvc;
using SIA.SchoolControlService.Application.UseCases.SubjectReferences;
using SIA.SchoolControlService.Contracts.Responses;

namespace SIA.SchoolControlService.Api.Controllers;

[ApiController]
[Route("api/subject-references")]
public sealed class SubjectReferencesController : ControllerBase
{
  private readonly GetSubjectReferenceUseCase _useCase;

  public SubjectReferencesController(GetSubjectReferenceUseCase useCase)
  {
    _useCase = useCase;
  }

  [HttpGet("{subjectId:guid}")]
  [ProducesResponseType(typeof(SubjectReferenceResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken)
  {
    try
    {
      var response = await _useCase.ExecuteAsync(subjectId, cancellationToken);

      if (response is null)
      {
        return NotFound(new
        {
          message =
                "No se encontró la referencia de la materia."
        });
      }

      return Ok(response);
    }
    catch (ArgumentException exception)
    {
      return BadRequest(new
      {
        message = exception.Message
      });
    }
  }
}
