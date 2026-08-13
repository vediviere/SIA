using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class ProvisionInitialAdministratorUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldProvisionAdministrator()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var role = new Role("Administrator", "Administrador institucional");
    var userDataStore = new FakeUserDataStore();
    var useCase = new ProvisionInitialAdministratorUseCase(userDataStore, new FakeRoleDataStore(role), new FakePasswordHasher());

    var response = await useCase.ExecuteAsync(
      tenantId,
      " ADMIN@INSTITUTION.EDU.MX ",
      "Temporary123!",
      correlationId,
      CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("admin@institution.edu.mx", response.Email);
    Assert.Equal(role.Id, response.RoleId);
    Assert.Equal("Administrator", response.RoleCode);
    Assert.True(response.MustChangePassword);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.True(userDataStore.UserWithRoleAdded);
    Assert.Equal("InitialAdministratorProvisioned", userDataStore.UserAuditAction);
    Assert.Null(userDataStore.ActorUserId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAdministratorWasAlreadyProvisioned_ShouldThrowConflict()
  {
    var role = new Role("Administrator", "Administrador institucional");
    var userDataStore = new FakeUserDataStore { HasRoleAssignmentResult = true };
    var useCase = new ProvisionInitialAdministratorUseCase(userDataStore, new FakeRoleDataStore(role), new FakePasswordHasher());

    await Assert.ThrowsAsync<InitialAdministratorException>(() => useCase.ExecuteAsync(
      Guid.NewGuid(),
      "admin@institution.edu.mx",
      "Temporary123!",
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowConflict()
  {
    var role = new Role("Administrator", "Administrador institucional");
    var userDataStore = new FakeUserDataStore { EmailExistsResult = true };
    var useCase = new ProvisionInitialAdministratorUseCase(userDataStore, new FakeRoleDataStore(role), new FakePasswordHasher());

    await Assert.ThrowsAsync<UserEmailAlreadyExistsException>(() => useCase.ExecuteAsync(
      Guid.NewGuid(),
      "admin@institution.edu.mx",
      "Temporary123!",
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenAdministratorRoleDoesNotExist_ShouldThrowNotFound()
  {
    var useCase = new ProvisionInitialAdministratorUseCase(new FakeUserDataStore(), new FakeRoleDataStore(null), new FakePasswordHasher());

    await Assert.ThrowsAsync<RoleNotFoundException>(() => useCase.ExecuteAsync(
      Guid.NewGuid(),
      "admin@institution.edu.mx",
      "Temporary123!",
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

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"HASHED:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"HASHED:{password}";
  }
}
