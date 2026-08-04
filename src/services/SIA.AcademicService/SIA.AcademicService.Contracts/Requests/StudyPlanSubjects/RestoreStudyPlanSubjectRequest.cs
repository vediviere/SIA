using System.Text.Json.Serialization;

namespace SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;

public sealed class RestoreStudyPlanSubjectRequest
{
    public Guid TenantId { get; set; }

    public Guid Id { get; set; }
}