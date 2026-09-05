namespace SIA.AdminBff.Infrastructure.Tenancy;

public interface ITenantContext
{
  Guid TenantId { get; }
}
