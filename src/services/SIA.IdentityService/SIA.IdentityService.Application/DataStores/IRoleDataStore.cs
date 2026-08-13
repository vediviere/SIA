using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.Interfaces.DataStores;

public interface IRoleDataStore
{
  Task<Role?> GetRoleByCodeAsync(string code, CancellationToken cancellationToken);

  Task<IReadOnlyList<Role>> GetActiveRolesAsync(Guid userId, CancellationToken cancellationToken);
}
