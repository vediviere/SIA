using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Models;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class LoginUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidCredentials_ShouldReturnToken()
  {
    var user = CreateUser();
    var role = new Role("Administrator", "Administrador institucional");
    var useCase = CreateUseCase(user, [role]);

    var result = await useCase.ExecuteAsync(new LoginRequest
    {
      Email = " ADMIN@INSTITUCION.EDU.MX ",
      Password = "Password123!"
    }, CancellationToken.None);

    Assert.Equal(user.Id, result.UserId);
    Assert.Equal(user.TenantId, result.TenantId);
    Assert.Equal("admin@institucion.edu.mx", result.Email);
    Assert.Equal("test-access-token", result.AccessToken);
    Assert.Equal("test-refresh-token", result.RefreshToken);
    Assert.Contains("Administrator", result.Roles);
  }

  [Fact]
  public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowUnauthorized()
  {
    var useCase = CreateUseCase(CreateUser(), []);

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(new LoginRequest
    {
      Email = "admin@institucion.edu.mx",
      Password = "Incorrect!"
    }, CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUnauthorized()
  {
    var useCase = CreateUseCase(null, []);

    await Assert.ThrowsAsync<InvalidCredentialsException>(() => useCase.ExecuteAsync(new LoginRequest
    {
      Email = "unknown@institucion.edu.mx",
      Password = "Password123!"
    }, CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenPasswordChangeIsRequired_ShouldThrowConflict()
  {
    var user = new User(Guid.NewGuid(), "admin@institucion.edu.mx", "HASHED:Temporary123!", mustChangePassword: true);
    var useCase = CreateUseCase(user, []);

    await Assert.ThrowsAsync<PasswordChangeException>(() => useCase.ExecuteAsync(new LoginRequest
    {
      Email = user.Email,
      Password = "Temporary123!"
    }, CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WithoutRoles_ShouldReturnTokenWithoutRoles()
  {
    var user = CreateUser();
    var useCase = CreateUseCase(user, []);

    var result = await useCase.ExecuteAsync(new LoginRequest
    {
      Email = user.Email,
      Password = "Password123!"
    }, CancellationToken.None);

    Assert.Empty(result.Roles);
    Assert.Equal("test-access-token", result.AccessToken);
  }

  private static User CreateUser()
  {
    return new User(Guid.NewGuid(), "admin@institucion.edu.mx", "HASHED:Password123!");
  }

  private static LoginUseCase CreateUseCase(User? user, IReadOnlyList<Role> roles)
  {
    return new LoginUseCase(
      new FakeUserDataStore(user),
      new FakeRoleDataStore(roles),
      new FakePasswordHasher(),
      new FakeTokenGenerator(),
      new FakeRefreshTokenService(),
      new FakeRefreshTokenDataStore(),
      new FakePermissionDataStore());
  }

  private sealed class FakeTokenGenerator : ITokenGenerator
  {
    public TokenResult Generate(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions)
      => new("test-access-token", DateTime.UtcNow.AddMinutes(5));
  }

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"HASHED:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"HASHED:{password}";
  }

  private sealed class FakeRoleDataStore : IRoleDataStore
  {
    private readonly IReadOnlyList<Role> _roles;

    public FakeRoleDataStore(IReadOnlyList<Role> roles)
    {
      _roles = roles;
    }

    public Task<Role?> GetRoleByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult<Role?>(null);

    public Task<IReadOnlyList<Role>> GetActiveRolesAsync(Guid userId, CancellationToken cancellationToken) => Task.FromResult(_roles);
  }

  private sealed class FakeRefreshTokenService : IRefreshTokenService
  {
    public RefreshTokenResult Generate() => new("test-refresh-token", "test-refresh-hash", DateTime.UtcNow.AddDays(7));

    public string Hash(string token) => $"HASH:{token}";
  }

  private sealed class FakeRefreshTokenDataStore : IRefreshTokenDataStore
  {
    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) => Task.FromResult<RefreshToken?>(null);

    public Task RotateAsync(RefreshToken currentToken, RefreshToken newToken, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
