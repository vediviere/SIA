namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;

public sealed record PersonActivatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public int Version { get; init; } = 1;
}