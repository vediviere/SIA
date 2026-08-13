using Microsoft.EntityFrameworkCore;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;

namespace SIA.IdentityService.Infrastructure.Persistence.DataStores;

public sealed class RoleDataStore : IRoleDataStore
{
  private readonly IdentityDbContext _dbContext;

  public RoleDataStore(IdentityDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Role?> GetRoleByCodeAsync(string code, CancellationToken cancellationToken)
  {
    var normalizedCode = code.Trim();

    return _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Code == normalizedCode, cancellationToken);
  }

  public async Task<IReadOnlyList<Role>> GetActiveRolesAsync(Guid userId, CancellationToken cancellationToken)
  {
    return await (
      from userRole in _dbContext.UserRoles
      join role in _dbContext.Roles on userRole.RoleId equals role.Id
      where userRole.UserId == userId && userRole.RevokedAtUtc == null
      select role).AsNoTracking().ToListAsync(cancellationToken);
  }
}
