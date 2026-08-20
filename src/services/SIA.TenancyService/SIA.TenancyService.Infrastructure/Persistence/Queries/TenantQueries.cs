using Microsoft.EntityFrameworkCore;
using SIA.TenancyService.Application.Interfaces.Queries;
using SIA.TenancyService.Domain.Entities;
using SIA.TenancyService.Infrastructure.Persistence.Contexts;

namespace SIA.TenancyService.Infrastructure.Persistence.Queries;

public sealed class TenantQueries : ITenantQueries
{
  private readonly TenancyDbContext _dbContext;

  public TenantQueries(TenancyDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Tenant?> GetByCodeAsync(string instituteCode, CancellationToken cancellationToken)
  {
    var normalizedCode = instituteCode.Trim().ToUpperInvariant();

    return _dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(tenant => tenant.InstituteCode == normalizedCode, cancellationToken);
  }
}
