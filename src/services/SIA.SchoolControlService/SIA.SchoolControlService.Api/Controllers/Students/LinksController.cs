using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Contracts.Responses.Students;

namespace SIA.SchoolControlService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/internal/students")]
public sealed class LinksController : ControllerBase
{
  private readonly LinkUseCase _useCase;

  public LinksController(LinkUseCase useCase)
  {
    _useCase = useCase;
  }

  [HttpPost("link")]
  [ProducesResponseType(typeof(LinkResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<LinkResponse>> Link([FromBody] LinkRequest request, CancellationToken cancellationToken)
  {
    ValidateContext(request);

    var response = await _useCase.ExecuteAsync(request, cancellationToken);

    return Ok(response);
  }

  private void ValidateContext(LinkRequest request)
  {
    var tenantValue = User.FindFirst("tenant_id")?.Value;
    var userValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    if (!Guid.TryParse(tenantValue, out var tenantId) || tenantId == Guid.Empty)
    {
      throw new UnauthorizedAccessException("El identificador de la institución no es válido.");
    }

    if (!Guid.TryParse(userValue, out var userId) || userId == Guid.Empty)
    {
      throw new UnauthorizedAccessException("El identificador del usuario no es válido.");
    }

    if (request.TenantId != tenantId || request.UserId != userId)
    {
      throw new UnauthorizedAccessException("Los datos de la solicitud no coinciden con el usuario autenticado.");
    }
  }
}
