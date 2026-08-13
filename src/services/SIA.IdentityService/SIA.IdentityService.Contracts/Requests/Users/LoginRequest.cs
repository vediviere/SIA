namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record LoginRequest
{
  public required string Email { get; init; }

  public required string Password { get; init; }
}
