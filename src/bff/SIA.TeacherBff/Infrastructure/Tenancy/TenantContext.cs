namespace SIA.TeacherBff.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
  private readonly IHttpContextAccessor _contextAccessor;

  public TenantContext(IHttpContextAccessor contextAccessor)
  {
    _contextAccessor = contextAccessor;
  }

  public Guid TenantId
  {
    get
    {
      var value = _contextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;

      if (!Guid.TryParse(value, out var tenantId) || tenantId == Guid.Empty)
      {
        throw new UnauthorizedAccessException("La identidad no contiene un TenantId válido.");
      }

      return tenantId;
    }
  }
}
