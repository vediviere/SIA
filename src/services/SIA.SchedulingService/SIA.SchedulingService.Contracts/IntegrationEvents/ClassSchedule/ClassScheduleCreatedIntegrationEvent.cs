namespace SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;

public sealed record ClassScheduleCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ClassScheduleId { get; init; }
    public required Guid OfferingId { get; init; }
    public required Guid ClassroomLabId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required string Day { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required bool Status { get; init; }
    public required int Version { get; init; }
}