namespace SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

public sealed record UpdateStudyPlanSubjectResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required Guid SubjectId { get; init; }
    public required int Semester { get; init; }
    public required int Credits { get; init; }
    public required bool IsRequired { get; init; }
    public required bool Status { get; init; }
    public required DateTime? UpdatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}