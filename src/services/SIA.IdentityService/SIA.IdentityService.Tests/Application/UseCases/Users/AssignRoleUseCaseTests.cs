using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class AssignRoleUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidRole_ShouldAssignRole()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var role = new Role("Teacher", "Docente");
    var userDataStore = new FakeUserDataStore(user);
    var useCase = new AssignRoleUseCase(userDataStore, new FakeRoleDataStore(role));

    await useCase.ExecuteAsync(
      user.Id,
      new AssignRoleRequest { RoleCode = "Teacher" },
      user.TenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None);

    Assert.True(userDataStore.RoleAdded);
  }

  [Fact]
  public async Task ExecuteAsync_WhenRoleAlreadyAssigned_ShouldThrowConflict()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var role = new Role("Teacher", "Docente");
    var userDataStore = new FakeUserDataStore(user) { HasActiveRoleResult = true };
    var useCase = new AssignRoleUseCase(userDataStore, new FakeRoleDataStore(role));

    await Assert.ThrowsAsync<RoleAssignmentException>(() => useCase.ExecuteAsync(
      user.Id,
      new AssignRoleRequest { RoleCode = "Teacher" },
      user.TenantId,
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserBelongsToAnotherTenant_ShouldThrowNotFound()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var role = new Role("Teacher", "Docente");
    var useCase = new AssignRoleUseCase(new FakeUserDataStore(user), new FakeRoleDataStore(role));

    await Assert.ThrowsAsync<UserNotFoundException>(() => useCase.ExecuteAsync(
      user.Id,
      new AssignRoleRequest { RoleCode = "Teacher" },
      Guid.NewGuid(),
      Guid.NewGuid(),
      Guid.NewGuid(),
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WithStudentRole_ShouldThrowInvalidStaffRole()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var useCase = new AssignRoleUseCase(new FakeUserDataStore(user), new FakeRoleDataStore(null));

    await Assert.ThrowsAsync<InvalidStaffRoleException>(() => useCase.ExecuteAsync(
      user.Id,
      new AssignRoleRequest { RoleCode = "Student" },
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
