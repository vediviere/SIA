namespace SIA.IdentityService.Application.Interfaces.Tenancy;

public interface ITenantResolver
{
  Task<Guid?> ResolveTenantIdAsync(string institutionCode, CancellationToken cancellationToken);
}
