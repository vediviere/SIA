using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class CreateStaffUserUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateUser()
  {
    var tenantId = Guid.NewGuid();
    var administratorUserId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var role = new Role("Teacher", "Docente");
    var userDataStore = new FakeUserDataStore();
    var useCase = new CreateStaffUserUseCase(userDataStore, new FakeRoleDataStore(role), new FakePasswordHasher());

    var response = await useCase.ExecuteAsync(new CreateStaffUserRequest
    {
      Email = " Teacher@Institution.edu.mx ",
      TemporaryPassword = "Temporary123!",
      RoleCode = "Teacher"
    }, tenantId, administratorUserId, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("teacher@institution.edu.mx", response.Email);
    Assert.Equal(role.Id, response.RoleId);
    Assert.Equal("Teacher", response.RoleCode);
    Assert.True(response.MustChangePassword);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.True(userDataStore.UserWithRoleAdded);
    Assert.Equal("StaffUserCreated", userDataStore.UserAuditAction);
    Assert.Equal(administratorUserId, userDataStore.ActorUserId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowConflict()
  {
    var userDataStore = new FakeUserDataStore { EmailExistsResult = true };
    var role = new Role("Teacher", "Docente");
    var useCase = new CreateStaffUserUseCase(userDataStore, new FakeRoleDataStore(role), new FakePasswordHasher());

    await Assert.ThrowsAsync<UserEmailAlreadyExistsException>(() => useCase.ExecuteAsync(new CreateStaffUserRequest
    {
      Email = "teacher@institution.edu.mx",
      TemporaryPassword = "Temporary123!",
      RoleCode = "Teacher"
    }, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenRoleDoesNotExist_ShouldThrowNotFound()
  {
    var useCase = new CreateStaffUserUseCase(new FakeUserDataStore(), new FakeRoleDataStore(null), new FakePasswordHasher());

    await Assert.ThrowsAsync<RoleNotFoundException>(() => useCase.ExecuteAsync(new CreateStaffUserRequest
    {
      Email = "teacher@institution.edu.mx",
      TemporaryPassword = "Temporary123!",
      RoleCode = "Teacher"
    }, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WithStudentRole_ShouldThrowInvalidStaffRole()
  {
    var useCase = new CreateStaffUserUseCase(new FakeUserDataStore(), new FakeRoleDataStore(null), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidStaffRoleException>(() => useCase.ExecuteAsync(new CreateStaffUserRequest
    {
      Email = "student@institution.edu.mx",
      TemporaryPassword = "Temporary123!",
      RoleCode = "Student"
    }, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
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
