namespace SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;

public sealed class StudyPlanSubjectUpdatedIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid StudyPlanSubjectId { get; init; }
    public Guid StudyPlanId { get; init; }
    public Guid SubjectId { get; init; }
    public int Semester { get; init; }
    public int Credits { get; init; }
    public bool IsRequired { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}