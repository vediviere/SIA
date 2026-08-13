namespace SIA.IdentityService.Domain.Entities;

public sealed class RefreshToken
{
  private RefreshToken() { }
  public RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("El usuario es obligatorio.", nameof(userId));
    }

    if (string.IsNullOrWhiteSpace(tokenHash))
    {
      throw new ArgumentException("El hash del token es obligatorio.", nameof(tokenHash));
    }

    Id = Guid.NewGuid();
    UserId = userId;
    TokenHash = tokenHash;
    CreatedAtUtc = DateTime.UtcNow;
    ExpiresAtUtc = expiresAtUtc;
  }

  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public string TokenHash { get; private set; } = string.Empty;
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime ExpiresAtUtc { get; private set; }
  public DateTime? RevokedAtUtc { get; private set; }

  public bool IsActive(DateTime utcNow) => RevokedAtUtc is null && ExpiresAtUtc > utcNow;

  public void Revoke()
  {
    if (RevokedAtUtc is null)
    {
      RevokedAtUtc = DateTime.UtcNow;
    }
  }
}
