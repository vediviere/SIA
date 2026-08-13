namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record AssignRoleRequest
{
  public required string RoleCode { get; init; }
}
