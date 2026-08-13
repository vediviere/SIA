using Microsoft.EntityFrameworkCore;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;

namespace SIA.IdentityService.Infrastructure.Persistence.DataStores;

public sealed class PermissionDataStore : IPermissionDataStore
{
  private readonly IdentityDbContext _dbContext;

  public PermissionDataStore(IdentityDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyList<Permission>> GetActivePermissionsAsync(Guid userId, CancellationToken cancellationToken)
  {
    return await (
  from userRole in _dbContext.UserRoles
  join rolePermission in _dbContext.RolePermissions on userRole.RoleId equals rolePermission.RoleId
  join permission in _dbContext.Permissions on rolePermission.PermissionId equals permission.Id
  where userRole.UserId == userId && userRole.RevokedAtUtc == null && rolePermission.RevokedAtUtc == null
  select permission).AsNoTracking().Distinct().ToListAsync(cancellationToken);
  }
}
