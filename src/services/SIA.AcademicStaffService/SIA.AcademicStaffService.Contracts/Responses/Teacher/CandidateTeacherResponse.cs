using SIA.AcademicStaffService.Contracts.Responses.TeacherCompetency;

namespace SIA.AcademicStaffService.Contracts.Responses.Teacher;

public sealed record CandidateTeacherResponse
{
    public required Guid TeacherId { get; init; }
    public required string ProfessionalProfile { get; init; }
    public Guid? ProgramId { get; init; }
    public required int ContractHours { get; init; }
    public required bool Status { get; init; }

    public required IEnumerable<CreateTeacherCompetencyResponse> Competencies { get; init; }
}