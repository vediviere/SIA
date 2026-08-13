namespace SIA.IdentityService.Contracts.IntegrationEvents.Users;

public static class UserIntegrationEventTypes
{
  public const string UserCreatedV1 = "UserCreatedIntegrationEvent.v1";
  public const string UserRoleAssignedV1 = "UserRoleAssignedIntegrationEvent.v1";
  public const string UserRoleRevokedV1 = "UserRoleRevokedIntegrationEvent.v1";
  public const string PasswordChangedV1 = "PasswordChangedIntegrationEvent.v1";
}
