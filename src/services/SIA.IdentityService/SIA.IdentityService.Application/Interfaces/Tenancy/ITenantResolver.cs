namespace SIA.IdentityService.Application.Interfaces.Tenancy;

public interface ITenantResolver
{
  Task<Guid?> ResolveTenantIdAsync(string instituteCode, string email, CancellationToken cancellationToken);
}
