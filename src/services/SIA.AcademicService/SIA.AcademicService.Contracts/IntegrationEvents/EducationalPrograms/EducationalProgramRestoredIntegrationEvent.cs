namespace SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;

public sealed record EducationalProgramRestoredIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid EducationalProgramId { get; init; }
    public int Version { get; init; } = 1;
}