namespace SIA.IdentityService.Contracts.IntegrationEvents.Users;

public sealed record PasswordChangedIntegrationEvent
{
  public required Guid EventId { get; init; }

  public required Guid CorrelationId { get; init; }

  public required DateTime OccurredAtUtc { get; init; }

  public required Guid TenantId { get; init; }

  public required Guid UserId { get; init; }

  public int Version { get; init; } = 1;
}
