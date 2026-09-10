namespace SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;

public sealed record ClassScheduleRestoredIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ClassScheduleId { get; init; }
    public required bool Status { get; init; }
    public required int Version { get; init; }
}