namespace SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;

public sealed class UpdateStudyPlanSubjectRequest
{
    public required int Semester { get; init; }
    public required int Credits { get; init; }
    public required bool IsRequired { get; init; }
}