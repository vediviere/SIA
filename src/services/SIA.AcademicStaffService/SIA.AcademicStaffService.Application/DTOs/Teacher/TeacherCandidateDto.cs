namespace SIA.AcademicStaffService.Application.DTOs.Teacher;

public sealed record TeacherCandidateDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid PersonId { get; init; }
    public string ProfessionalProfile { get; init; } = default!;
    public string ContractType { get; init; } = default!;
    public int ContractHours { get; init; }
    public Guid? ProgramId { get; init; }
    public bool Status { get; init; }

    public IEnumerable<TeacherCompetencyDto> Competencies { get; init; } = Enumerable.Empty<TeacherCompetencyDto>();
}

public sealed record TeacherCompetencyDto
{
    public Guid TeacherCompetencyId { get; init; }
    public Guid CompetencyId { get; init; }
    public string Level { get; init; } = default!;
    public string Stage { get; init; } = default!;
    public bool Status { get; init; }
}