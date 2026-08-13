using Microsoft.EntityFrameworkCore;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;

namespace SIA.IdentityService.Infrastructure.Persistence.DataStores;

public sealed class RefreshTokenDataStore : IRefreshTokenDataStore
{
  private readonly IdentityDbContext _dbContext;

  public RefreshTokenDataStore(IdentityDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
  {
    await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
  {
    return _dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
  }

  public async Task RotateAsync(RefreshToken currentToken, RefreshToken newToken, CancellationToken cancellationToken)
  {
    await _dbContext.RefreshTokens.AddAsync(newToken, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
  {
    return _dbContext.SaveChangesAsync(cancellationToken);
  }
}
