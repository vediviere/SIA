namespace SIA.SchedulingService.Contracts.IntegrationEvents.Group;

public sealed class GroupActivateIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid GroupId { get; init; }

    public required Guid EducationalProgramId { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}