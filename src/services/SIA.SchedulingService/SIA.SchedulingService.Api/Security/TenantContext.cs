using SIA.SchedulingService.Application.Interfaces;

namespace SIA.SchedulingService.Api.Security
{
    public class TenantContext : ITenantContext
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
}
