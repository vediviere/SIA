using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class ChangePasswordUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidPassword_ShouldChangePassword()
  {
    var user = CreateUser();
    var dataStore = new FakeUserDataStore(user);
    var useCase = new ChangePasswordUseCase(dataStore, new FakePasswordHasher());

    await useCase.ExecuteAsync(user.Id, new ChangePasswordRequest
    {
      CurrentPassword = "Current123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None);

    Assert.Equal("HASHED:NewPassword123!", user.PasswordHash);
    Assert.True(dataStore.PasswordUpdated);
  }

  [Fact]
  public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowUnauthorized()
  {
    var user = CreateUser();
    var useCase = new ChangePasswordUseCase(new FakeUserDataStore(user), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(user.Id, new ChangePasswordRequest
    {
      CurrentPassword = "Incorrect!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUnauthorized()
  {
    var useCase = new ChangePasswordUseCase(new FakeUserDataStore(), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(Guid.NewGuid(), new ChangePasswordRequest
    {
      CurrentPassword = "Current123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowUnauthorized()
  {
    var user = CreateUser();
    user.Deactivate();

    var useCase = new ChangePasswordUseCase(new FakeUserDataStore(user), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(user.Id, new ChangePasswordRequest
    {
      CurrentPassword = "Current123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  private static User CreateUser() => new(Guid.NewGuid(), "user@institucion.edu.mx", "HASHED:Current123!");

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"HASHED:{password}";
    public bool Verify(string passwordHash, string password) => passwordHash == $"HASHED:{password}";
  }
}
