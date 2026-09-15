using SIA.IdentityService.Contracts.Enums;

namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record AssignRoleRequest
{
  public required RoleCode RoleCode { get; init; }
}
