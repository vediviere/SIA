using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Interfaces.Tenancy;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class SelfRegisterUseCaseTest
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateUserWithoutRoles()
  {
    var tenantId = Guid.NewGuid();
    var correlationId = Guid.NewGuid();
    var userDataStore = new FakeUserDataStore();
    var useCase = new SelfRegisterUseCase(userDataStore, new FakeTenantResolver(tenantId), new FakePasswordHasher());

    var response = await useCase.ExecuteAsync(new SelfRegisterRequest
    {
      InstituteCode = " institution ",
      Email = " Student@Institution.edu.mx ",
      Password = "Password123!"
    }, correlationId, CancellationToken.None);

    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("student@institution.edu.mx", response.Email);
    Assert.False(response.MustChangePassword);
    Assert.Equal(correlationId, response.CorrelationId);
    Assert.True(userDataStore.UserAdded);
    Assert.False(userDataStore.UserWithRoleAdded);
    Assert.False(userDataStore.RoleAdded);
    Assert.Equal("UserSelfRegistered", userDataStore.UserAuditAction);
  }

  [Fact]
  public async Task ExecuteAsync_WhenInstitutionDoesNotExist_ShouldThrowNotFound()
  {
    var useCase = new SelfRegisterUseCase(new FakeUserDataStore(), new FakeTenantResolver(null), new FakePasswordHasher());

    await Assert.ThrowsAsync<InstitutionNotFoundException>(() => useCase.ExecuteAsync(new SelfRegisterRequest
    {
      InstituteCode = "UNKNOWN",
      Email = "student@institution.edu.mx",
      Password = "Password123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowConflict()
  {
    var userDataStore = new FakeUserDataStore { EmailExistsResult = true };
    var useCase = new SelfRegisterUseCase(userDataStore, new FakeTenantResolver(Guid.NewGuid()), new FakePasswordHasher());

    await Assert.ThrowsAsync<UserEmailAlreadyExistsException>(() => useCase.ExecuteAsync(new SelfRegisterRequest
    {
      InstituteCode = "INSTITUTION",
      Email = "student@institution.edu.mx",
      Password = "Password123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  private sealed class FakeTenantResolver : ITenantResolver
  {
    private readonly Guid? _tenantId;

    public FakeTenantResolver(Guid? tenantId)
    {
      _tenantId = tenantId;
    }

    public Task<Guid?> ResolveTenantIdAsync(string instituteCode, string email, CancellationToken cancellationToken) => Task.FromResult(_tenantId);
  }

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"HASHED:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"HASHED:{password}";
  }
}
