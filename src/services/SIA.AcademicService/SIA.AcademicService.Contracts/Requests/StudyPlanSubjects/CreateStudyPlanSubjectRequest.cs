using System.Text.Json.Serialization;

namespace SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;

public sealed record CreateStudyPlanSubjectRequest
{
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required Guid SubjectId { get; init; }
    public required int Semester { get; init; }
    public required int Credits { get; init; }
    public required bool IsRequired { get; init; }
}