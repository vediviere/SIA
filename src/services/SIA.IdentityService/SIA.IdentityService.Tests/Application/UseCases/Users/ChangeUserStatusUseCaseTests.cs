using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class ChangeUserStatusUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_Lock_ShouldLockUser()
  {
    var user = CreateUser();
    var dataStore = new FakeUserDataStore(user);
    var useCase = new ChangeUserStatusUseCase(dataStore);

    await useCase.ExecuteAsync(user.Id, UserStatus.Locked, user.TenantId, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

    Assert.Equal(UserStatus.Locked, user.Status);
    Assert.True(dataStore.StatusChanged);
  }

  [Fact]
  public async Task ExecuteAsync_Deactivate_ShouldDeactivateUser()
  {
    var user = CreateUser();
    var dataStore = new FakeUserDataStore(user);
    var useCase = new ChangeUserStatusUseCase(dataStore);

    await useCase.ExecuteAsync(user.Id, UserStatus.Inactive, user.TenantId, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

    Assert.Equal(UserStatus.Inactive, user.Status);
    Assert.True(dataStore.StatusChanged);
  }

  [Fact]
  public async Task ExecuteAsync_Activate_ShouldActivateUser()
  {
    var user = CreateUser();
    user.Lock();

    var dataStore = new FakeUserDataStore(user);
    var useCase = new ChangeUserStatusUseCase(dataStore);

    await useCase.ExecuteAsync(user.Id, UserStatus.Active, user.TenantId, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

    Assert.Equal(UserStatus.Active, user.Status);
    Assert.True(dataStore.StatusChanged);
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserBelongsToAnotherTenant_ShouldThrowNotFound()
  {
    var user = CreateUser();
    var useCase = new ChangeUserStatusUseCase(new FakeUserDataStore(user));

    await Assert.ThrowsAsync<UserNotFoundException>(() =>
      useCase.ExecuteAsync(user.Id, UserStatus.Locked, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
  }

  private static User CreateUser() => new(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
}
