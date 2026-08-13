namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record CreateStaffUserRequest
{
  public required string Email { get; init; }

  public required string TemporaryPassword { get; init; }

  public required string RoleCode { get; init; }
}
