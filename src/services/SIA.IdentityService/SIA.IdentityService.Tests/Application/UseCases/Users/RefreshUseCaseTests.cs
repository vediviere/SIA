using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Models;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Tests.Common.Fakes;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class RefreshUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidToken_ShouldRotateToken()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var currentToken = new RefreshToken(user.Id, "HASH:old-token", DateTime.UtcNow.AddDays(7));
    var dataStore = new FakeRefreshTokenDataStore(currentToken);

    var result = await CreateUseCase(user, dataStore).ExecuteAsync(
      new RefreshRequest { RefreshToken = "old-token" },
      CancellationToken.None);

    Assert.Equal("new-refresh-token", result.RefreshToken);
    Assert.NotNull(currentToken.RevokedAtUtc);
    Assert.True(dataStore.Rotated);
  }

  [Fact]
  public async Task ExecuteAsync_WhenTokenDoesNotExist_ShouldThrowUnauthorized()
  {
    var useCase = CreateUseCase(null, new FakeRefreshTokenDataStore(null));

    await Assert.ThrowsAsync<RefreshTokenException>(() => useCase.ExecuteAsync(
      new RefreshRequest { RefreshToken = "invalid" },
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenTokenIsRevoked_ShouldThrowUnauthorized()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var token = new RefreshToken(user.Id, "HASH:old-token", DateTime.UtcNow.AddDays(7));
    token.Revoke();

    var useCase = CreateUseCase(user, new FakeRefreshTokenDataStore(token));

    await Assert.ThrowsAsync<RefreshTokenException>(() => useCase.ExecuteAsync(
      new RefreshRequest { RefreshToken = "old-token" },
      CancellationToken.None));
  }

  [Fact]
  public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowUnauthorized()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    user.Deactivate();

    var token = new RefreshToken(user.Id, "HASH:old-token", DateTime.UtcNow.AddDays(7));
    var useCase = CreateUseCase(user, new FakeRefreshTokenDataStore(token));

    await Assert.ThrowsAsync<RefreshTokenException>(() => useCase.ExecuteAsync(
      new RefreshRequest { RefreshToken = "old-token" },
      CancellationToken.None));
  }

  private static RefreshUseCase CreateUseCase(User? user, FakeRefreshTokenDataStore refreshTokenDataStore)
  {
    return new RefreshUseCase(
      refreshTokenDataStore,
      new FakeRefreshTokenService(),
      new FakeUserDataStore(user),
      new FakeRoleDataStore(),
      new FakeTokenGenerator(),
      new FakePermissionDataStore());
  }

  private sealed class FakeRefreshTokenService : IRefreshTokenService
  {
    public RefreshTokenResult Generate() => new("new-refresh-token", "HASH:new-refresh-token", DateTime.UtcNow.AddDays(7));

    public string Hash(string token) => $"HASH:{token}";
  }

  private sealed class FakeRefreshTokenDataStore : IRefreshTokenDataStore
  {
    private readonly RefreshToken? _token;

    public bool Rotated { get; private set; }

    public FakeRefreshTokenDataStore(RefreshToken? token)
    {
      _token = token;
    }

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) => Task.FromResult(_token);

    public Task RotateAsync(RefreshToken currentToken, RefreshToken newToken, CancellationToken cancellationToken)
    {
      Rotated = true;
      return Task.CompletedTask;
    }

    public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken) => Task.CompletedTask;
  }

  private sealed class FakeRoleDataStore : IRoleDataStore
  {
    public Task<Role?> GetRoleByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult<Role?>(null);

    public Task<IReadOnlyList<Role>> GetActiveRolesAsync(Guid userId, CancellationToken cancellationToken) =>
      Task.FromResult<IReadOnlyList<Role>>(Array.Empty<Role>());
  }

  private sealed class FakeTokenGenerator : ITokenGenerator
  {
    public TokenResult Generate(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions) => new("new-access-token", DateTime.UtcNow.AddMinutes(5));
  }


}
