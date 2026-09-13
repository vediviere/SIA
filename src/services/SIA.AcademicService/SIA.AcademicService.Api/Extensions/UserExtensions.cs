using System.Security.Claims;

namespace SIA.AcademicService.Api.Extensions;

public static class UserExtensions
{
    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var tenantClaim = user.FindFirst("tenantId")?.Value
                       ?? user.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrWhiteSpace(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
        {
            throw new UnauthorizedAccessException("El token de acceso no contiene un TenantId válido para operar.");
        }

        return tenantId;
    }
}