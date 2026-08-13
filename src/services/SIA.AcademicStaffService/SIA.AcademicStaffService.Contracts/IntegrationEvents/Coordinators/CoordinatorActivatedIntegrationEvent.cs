namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;

public sealed record CoordinatorActivatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid CoordinatorId { get; init; }

    public int Version { get; init; } = 1;
}