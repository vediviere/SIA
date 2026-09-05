namespace SIA.AdminBff.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public TenantContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid TenantId
  {
    get
    {
      var tenantIdValue = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;

      if (!Guid.TryParse(tenantIdValue, out var tenantId) || tenantId == Guid.Empty)
      {
        throw new UnauthorizedAccessException("La identidad de la sesión no contiene un TenantId válido.");
      }

      return tenantId;
    }
  }
}
