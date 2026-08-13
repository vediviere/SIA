using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Models;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Application.UseCases.Users;

public sealed class LogoutUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithActiveToken_ShouldRevokeToken()
  {
    var token = new RefreshToken(Guid.NewGuid(), "HASH:refresh-token", DateTime.UtcNow.AddDays(7));
    var dataStore = new FakeRefreshTokenDataStore(token);
    var useCase = new LogoutUseCase(dataStore, new FakeRefreshTokenService());

    await useCase.ExecuteAsync(new LogoutRequest { RefreshToken = "refresh-token" }, CancellationToken.None);

    Assert.NotNull(token.RevokedAtUtc);
    Assert.True(dataStore.Revoked);
  }

  [Fact]
  public async Task ExecuteAsync_WhenTokenDoesNotExist_ShouldCompleteWithoutError()
  {
    var dataStore = new FakeRefreshTokenDataStore(null);
    var useCase = new LogoutUseCase(dataStore, new FakeRefreshTokenService());

    await useCase.ExecuteAsync(new LogoutRequest { RefreshToken = "unknown-token" }, CancellationToken.None);

    Assert.False(dataStore.Revoked);
  }

  private sealed class FakeRefreshTokenService : IRefreshTokenService
  {
    public RefreshTokenResult Generate() => new("token", "HASH:token", DateTime.UtcNow.AddDays(7));

    public string Hash(string token) => $"HASH:{token}";
  }

  private sealed class FakeRefreshTokenDataStore : IRefreshTokenDataStore
  {
    private readonly RefreshToken? _token;

    public bool Revoked { get; private set; }

    public FakeRefreshTokenDataStore(RefreshToken? token)
    {
      _token = token;
    }

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) => Task.FromResult(_token);

    public Task RotateAsync(RefreshToken currentToken, RefreshToken newToken, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
      Revoked = true;
      return Task.CompletedTask;
    }
  }
}
