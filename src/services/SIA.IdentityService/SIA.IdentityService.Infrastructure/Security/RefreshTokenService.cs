using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Models;
using System.Security.Cryptography;
using System.Text;

namespace SIA.IdentityService.Infrastructure.Security;

public sealed class RefreshTokenService : IRefreshTokenService
{
  private readonly TokenSettings _settings;

  public RefreshTokenService(TokenSettings settings)
  {
    _settings = settings;
  }

  public RefreshTokenResult Generate()
  {
    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    var tokenHash = Hash(token);
    var expiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

    return new RefreshTokenResult(token, tokenHash, expiresAtUtc);
  }

  public string Hash(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
    {
      throw new ArgumentException("El refresh token es obligatorio.", nameof(token));
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

    return Convert.ToHexString(hash);
  }
}
