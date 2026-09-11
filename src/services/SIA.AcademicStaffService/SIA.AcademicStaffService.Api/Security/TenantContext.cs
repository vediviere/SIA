using SIA.AcademicStaffService.Application.Interfaces;

namespace SIA.AcademicStaffService.Api.Security;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; }

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var tenantClaim = user?.FindFirst("tenant_id")?.Value;

        if (Guid.TryParse(tenantClaim, out var tenantId))
        {
            TenantId = tenantId;
        }
        else
        {
            throw new UnauthorizedAccessException("El token JWT no contiene un TenantId válido o está ausente.");
        }
    }
}