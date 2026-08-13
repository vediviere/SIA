namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record SetInitialPasswordRequest
{
  public required string Email { get; init; }

  public required string TemporaryPassword { get; init; }

  public required string NewPassword { get; init; }
}
