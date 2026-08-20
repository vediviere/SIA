using SIA.TenancyService.Domain.Entities;

namespace SIA.TenancyService.Application.Interfaces.Queries;

public interface ITenantQueries
{
  Task<Tenant?> GetByCodeAsync(string instituteCode, CancellationToken cancellationToken);
}
