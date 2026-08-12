namespace SIA.SchedulingService.Contracts.IntegrationEvents.Building;

public sealed record BuildingActivatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid BuildingId { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}