using SIA.IdentityService.Contracts.Enums;

namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record CreateStaffUserRequest
{
  public required string Email { get; init; }
  public required string TemporaryPassword { get; init; }
  public required RoleCode RoleCode { get; init; }
}
