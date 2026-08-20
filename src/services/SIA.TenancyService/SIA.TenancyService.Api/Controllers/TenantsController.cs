using Microsoft.AspNetCore.Mvc;
using SIA.TenancyService.Application.UseCases.Tenants;
using SIA.TenancyService.Contracts.Requests.Tenants;
using SIA.TenancyService.Contracts.Responses.Tenants;

namespace SIA.TenancyService.Api.Controllers;

[ApiController]
[Route("api/tenants")]
public sealed class TenantsController : ControllerBase
{
  private readonly ResolveTenantUseCase _resolveTenantUseCase;

  public TenantsController(ResolveTenantUseCase resolveTenantUseCase)
  {
    _resolveTenantUseCase = resolveTenantUseCase;
  }

  [HttpPost("resolve")]
  [ProducesResponseType(typeof(ResolveTenantResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<ResolveTenantResponse>> Resolve([FromBody] ResolveTenantRequest request, CancellationToken cancellationToken)
  {
    var response = await _resolveTenantUseCase.ExecuteAsync(request, cancellationToken);

    return Ok(response);
  }
}
