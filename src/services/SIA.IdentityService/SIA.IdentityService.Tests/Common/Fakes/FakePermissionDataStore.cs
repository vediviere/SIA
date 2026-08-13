using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Common.Fakes;

public sealed class FakePermissionDataStore : IPermissionDataStore
{
  public IReadOnlyList<Permission> Permissions { get; set; } = [];

  public Task<IReadOnlyList<Permission>> GetActivePermissionsAsync(Guid userId, CancellationToken cancellationToken) => Task.FromResult(Permissions);
}
