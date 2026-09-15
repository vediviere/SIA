using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.IdentityService.Contracts.IntegrationEvents.Users;

namespace SIA.IdentityService.Infrastructure.MessageBus;

public static class IdentityOutboxRegistry
{
  public static OutboxEventRegistry Create()
  {
    return new OutboxEventRegistry()
      .Register<UserCreatedIntegrationEvent>(UserIntegrationEventTypes.UserCreatedV1)
      .Register<UserRoleAssignedIntegrationEvent>(UserIntegrationEventTypes.UserRoleAssignedV1)
      .Register<UserRoleRevokedIntegrationEvent>(UserIntegrationEventTypes.UserRoleRevokedV1)
      .Register<PasswordChangedIntegrationEvent>(UserIntegrationEventTypes.PasswordChangedV1);
  }
}
