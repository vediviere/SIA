using SIA.WorkflowService.Application.Interfaces;

namespace SIA.WorkflowService.Api.Security;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; }

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var tenantClaim = user?.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantClaim, out var tenantId) || tenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "El token JWT no contiene un TenantId válido o está ausente.");
        }

        TenantId = tenantId;
    }
}