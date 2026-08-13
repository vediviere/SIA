using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Contracts.Requests.Users;

namespace SIA.IdentityService.Application.UseCases.Users;

public sealed class LogoutUseCase
{
  private readonly IRefreshTokenDataStore _refreshTokenDataStore;
  private readonly IRefreshTokenService _refreshTokenService;

  public LogoutUseCase(IRefreshTokenDataStore refreshTokenDataStore, IRefreshTokenService refreshTokenService)
  {
    _refreshTokenDataStore = refreshTokenDataStore;
    _refreshTokenService = refreshTokenService;
  }

  public async Task ExecuteAsync(LogoutRequest request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      throw new ArgumentException("El refresh token es obligatorio.", nameof(request.RefreshToken));
    }

    var tokenHash = _refreshTokenService.Hash(request.RefreshToken);
    var refreshToken = await _refreshTokenDataStore.GetByHashAsync(tokenHash, cancellationToken);

    if (refreshToken is null || !refreshToken.IsActive(DateTime.UtcNow))
    {
      return;
    }

    refreshToken.Revoke();

    await _refreshTokenDataStore.RevokeAsync(refreshToken, cancellationToken);
  }
}
