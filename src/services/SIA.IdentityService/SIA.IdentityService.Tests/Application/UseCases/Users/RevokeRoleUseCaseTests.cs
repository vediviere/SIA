using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class RevokeRoleUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithActiveRole_ShouldRevokeRole()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var role = new Role("CareerHead", "Jefe de carrera");
    var userRole = new UserRole(user.Id, role.Id);

    var userDataStore = new FakeUserDataStore(user) { ActiveUserRole = userRole };
    var useCase = new RevokeRoleUseCase(userDataStore, new FakeRoleDataStore(role));

    await useCase.ExecuteAsync(
      user.Id,
      "CareerHead",
      user.TenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None);

    Assert.NotNull(userRole.RevokedAtUtc);
    Assert.True(userDataStore.RoleRevoked);
  }

  [Fact]
  public async Task ExecuteAsync_WithoutActiveRole_ShouldThrowConflict()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var role = new Role("CareerHead", "Jefe de carrera");
    var useCase = new RevokeRoleUseCase(new FakeUserDataStore(user), new FakeRoleDataStore(role));

    await Assert.ThrowsAsync<RoleRevocationException>(() => useCase.ExecuteAsync(
      user.Id,
      "CareerHead",
      user.TenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserBelongsToAnotherTenant_ShouldThrowNotFound()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var useCase = new RevokeRoleUseCase(new FakeUserDataStore(user), new FakeRoleDataStore(null));

    await Assert.ThrowsAsync<UserNotFoundException>(() => useCase.ExecuteAsync(
      user.Id,
      "CareerHead",
      Guid.NewGuid(),
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenRoleDoesNotExist_ShouldThrowNotFound()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var useCase = new RevokeRoleUseCase(new FakeUserDataStore(user), new FakeRoleDataStore(null));

    await Assert.ThrowsAsync<RoleNotFoundException>(() => useCase.ExecuteAsync(
      user.Id,
      "Unknown",
      user.TenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None));
  }

  private sealed class FakeRoleDataStore : IRoleDataStore
  {
    private readonly Role? _role;

    public FakeRoleDataStore(Role? role)
    {
      _role = role;
    }

    public Task<Role?> GetRoleByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult(_role);

    public Task<IReadOnlyList<Role>> GetActiveRolesAsync(Guid userId, CancellationToken cancellationToken) =>
      Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
  }
}
