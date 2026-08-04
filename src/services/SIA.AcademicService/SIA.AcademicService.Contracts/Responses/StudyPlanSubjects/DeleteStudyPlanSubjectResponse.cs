namespace SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

public sealed class DeleteStudyPlanSubjectResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}