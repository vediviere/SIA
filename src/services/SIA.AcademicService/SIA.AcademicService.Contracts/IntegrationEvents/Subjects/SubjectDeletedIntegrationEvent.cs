namespace SIA.AcademicService.Contracts.IntegrationEvents.Subjects;

public sealed class SubjectDeletedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid SubjectId { get; init; }
    public int Version { get; init; } = 1;
}