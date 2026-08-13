using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class SetInitialPasswordUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldChangePassword()
  {
    var user = new User(Guid.NewGuid(), "admin@institution.edu.mx", "HASHED:Temporary123!", mustChangePassword: true);
    var userDataStore = new FakeUserDataStore(user);
    var useCase = new SetInitialPasswordUseCase(userDataStore, new FakePasswordHasher());

    await useCase.ExecuteAsync(new SetInitialPasswordRequest
    {
      Email = "admin@institution.edu.mx",
      TemporaryPassword = "Temporary123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None);

    Assert.Equal("HASHED:NewPassword123!", user.PasswordHash);
    Assert.False(user.MustChangePassword);
    Assert.True(userDataStore.PasswordUpdated);
  }

  [Fact]
  public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowUnauthorized()
  {
    var user = new User(Guid.NewGuid(), "admin@institution.edu.mx", "HASHED:Temporary123!", mustChangePassword: true);
    var useCase = new SetInitialPasswordUseCase(new FakeUserDataStore(user), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(new SetInitialPasswordRequest
    {
      Email = "admin@institution.edu.mx",
      TemporaryPassword = "Incorrect!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUnauthorized()
  {
    var useCase = new SetInitialPasswordUseCase(new FakeUserDataStore(), new FakePasswordHasher());

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(new SetInitialPasswordRequest
    {
      Email = "unknown@institution.edu.mx",
      TemporaryPassword = "Temporary123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenInitialPasswordWasAlreadyChanged_ShouldThrowConflict()
  {
    var user = new User(Guid.NewGuid(), "admin@institution.edu.mx", "HASHED:Password123!");
    var useCase = new SetInitialPasswordUseCase(new FakeUserDataStore(user), new FakePasswordHasher());

    await Assert.ThrowsAsync<InitialPasswordException>(() => useCase.ExecuteAsync(new SetInitialPasswordRequest
    {
      Email = "admin@institution.edu.mx",
      TemporaryPassword = "Password123!",
      NewPassword = "NewPassword123!"
    }, Guid.NewGuid(), CancellationToken.None));
  }

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"HASHED:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"HASHED:{password}";
  }
}
