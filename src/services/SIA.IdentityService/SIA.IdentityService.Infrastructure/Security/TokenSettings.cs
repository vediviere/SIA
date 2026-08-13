namespace SIA.IdentityService.Infrastructure.Security;

public sealed class TokenSettings
{
  public required string Issuer { get; init; }
  public required string Audience { get; init; }
  public required string SigningKey { get; init; }
  public required int ExpirationMinutes { get; init; }
  public required int RefreshTokenExpirationDays { get; init; }
}
