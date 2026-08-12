namespace SIA.SchedulingService.Contracts.IntegrationEvents.Group;

public sealed record GroupCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid GroupId { get; init; }

    public required Guid EducationalProgramId { get; init; }

    public required string GroupName { get; init; }

    public required string Shift { get; init; }

    public required int Capacity { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}