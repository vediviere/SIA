namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionHeads;

public sealed record DivisionHeadDeactivatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid DivisionHeadId { get; init; }
    public int Version { get; init; } = 1;
}