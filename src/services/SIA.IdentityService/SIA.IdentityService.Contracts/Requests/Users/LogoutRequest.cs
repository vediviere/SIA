namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record LogoutRequest
{
  public required string RefreshToken { get; init; }
}
