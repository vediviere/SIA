namespace SIA.SchedulingService.Contracts.IntegrationEvents;
public sealed record TeachingSupportHoursUpdatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid SupportHourId { get; init; }
    public required int Hours { get; init; }
    public required bool Status { get; init; }
    public int Version { get; init; } = 1;
}