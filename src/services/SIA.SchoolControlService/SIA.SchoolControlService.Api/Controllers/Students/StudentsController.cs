using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Contracts.Responses.Students;

namespace SIA.SchoolControlService.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrator")]
[Route("api/students")]
public sealed class StudentsController : ControllerBase
{
  private readonly CreateUseCase _createUseCase;
  private readonly GetByNumberUseCase _getByNumberUseCase;

  public StudentsController(CreateUseCase createUseCase, GetByNumberUseCase getByNumberUseCase)
  {
    _createUseCase = createUseCase;
    _getByNumberUseCase = getByNumberUseCase;
  }

  [HttpPost]
  [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<StudentResponse>> Create([FromBody] CreateRequest request, CancellationToken cancellationToken)
  {
    var response = await _createUseCase.ExecuteAsync(GetTenantId(), request, cancellationToken);

    return CreatedAtAction(nameof(GetByNumber), new { studentNumber = response.StudentNumber }, response);
  }

  [HttpGet("by-number/{studentNumber}")]
  [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<StudentResponse>> GetByNumber(string studentNumber, CancellationToken cancellationToken)
  {
    var response = await _getByNumberUseCase.ExecuteAsync(GetTenantId(), studentNumber, cancellationToken);

    if (response is null)
    {
      return NotFound(new
      {
        message = "No se encontró el estudiante."
      });
    }

    return Ok(response);
  }

  private Guid GetTenantId()
  {
    var tenantIdValue = User.FindFirst("tenant_id")?.Value;

    if (!Guid.TryParse(tenantIdValue, out var tenantId) || tenantId == Guid.Empty)
    {
      throw new UnauthorizedAccessException("El identificador de la institución no es válido.");
    }

    return tenantId;
  }
}
