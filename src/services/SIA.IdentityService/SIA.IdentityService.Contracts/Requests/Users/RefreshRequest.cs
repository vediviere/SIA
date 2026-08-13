namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record RefreshRequest
{
  public required string RefreshToken { get; init; }
}
