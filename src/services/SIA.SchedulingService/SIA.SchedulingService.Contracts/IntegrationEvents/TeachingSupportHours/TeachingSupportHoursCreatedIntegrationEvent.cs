namespace SIA.SchedulingService.Contracts.IntegrationEvents;
public sealed record TeachingSupportHoursCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid SupportHourId { get; init; }
    public required Guid ActivityId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required int Hours { get; init; }
    public required bool Status { get; init; }
    public int Version { get; init; } = 1;
}