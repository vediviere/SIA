namespace SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;

public sealed record StudyPlanSubjectDeletedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanSubjectId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required Guid SubjectId { get; init; }
    public required bool Status { get; init; }
    public int Version { get; init; } = 1;
}