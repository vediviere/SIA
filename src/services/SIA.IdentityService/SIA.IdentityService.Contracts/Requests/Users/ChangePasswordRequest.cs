namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record ChangePasswordRequest
{
  public required string CurrentPassword { get; init; }
  public required string NewPassword { get; init; }
}
