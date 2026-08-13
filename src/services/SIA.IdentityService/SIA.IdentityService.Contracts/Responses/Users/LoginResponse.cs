namespace SIA.IdentityService.Contracts.Responses.Users;

public sealed record LoginResponse
{
  public required Guid UserId { get; init; }

  public required Guid TenantId { get; init; }

  public required string Email { get; init; }

  public required string AccessToken { get; init; }

  public required DateTime AccessTokenExpiresAtUtc { get; init; }

  public required string RefreshToken { get; init; }

  public required DateTime RefreshTokenExpiresAtUtc { get; init; }

  public required IReadOnlyCollection<string> Roles { get; init; }
}
