namespace SIA.AcademicStaffService.Application.DTOs.TeacherCompetency;
public sealed record TeacherCompetencyDto
{
    public Guid TeacherCompetencyId { get; init; }
    public Guid CompetencyId { get; init; }
    public string Level { get; init; } = default!;
    public string Stage { get; init; } = default!;
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}