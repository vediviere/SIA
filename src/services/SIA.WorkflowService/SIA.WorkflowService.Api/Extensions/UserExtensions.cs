using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SIA.WorkflowService.Api.Extensions;

public static class UserExtensions
{
  public static Guid GetUserId(this ClaimsPrincipal user)
  {
    var value = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    if (!Guid.TryParse(value, out var userId))
    {
      throw new UnauthorizedAccessException("El token no contiene un identificador de usuario válido.");
    }

    return userId;
  }

  public static Guid GetTenantId(this ClaimsPrincipal user)
  {
    var value = user.FindFirst("tenant_id")?.Value;

    if (!Guid.TryParse(value, out var tenantId))
    {
      throw new UnauthorizedAccessException("El token no contiene un TenantId válido.");
    }

    return tenantId;
  }
}
