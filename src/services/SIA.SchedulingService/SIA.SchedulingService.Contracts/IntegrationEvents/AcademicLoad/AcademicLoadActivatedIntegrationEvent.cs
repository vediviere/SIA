namespace SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;

public sealed record AcademicLoadActivatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required bool Status { get; init; }
    public int Version { get; init; } = 1;
}
