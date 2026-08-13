using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Application.Interfaces.DataStores;

public interface IRefreshTokenDataStore
{
  Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
  Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);
  Task RotateAsync(RefreshToken currentToken, RefreshToken newToken, CancellationToken cancellationToken);
  Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}
