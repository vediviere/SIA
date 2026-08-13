using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.Interfaces.DataStores;

public interface IPermissionDataStore
{
  Task<IReadOnlyList<Permission>> GetActivePermissionsAsync(Guid userId, CancellationToken cancellationToken);
}
