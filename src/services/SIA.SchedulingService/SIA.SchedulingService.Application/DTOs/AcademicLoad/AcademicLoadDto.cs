
namespace SIA.SchedulingService.Application.DTOs.AcademicLoad;

public sealed record AcademicLoadDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid TeacherId { get; init; }
    public Guid DivisionId { get; init; }
    public Guid AcademicPeriodId { get; init; }
    public string OfficialLetterNumber { get; init; } = string.Empty;
    public DateTime ProposedDate { get; init; }
    public int ClassHours { get; init; }
    public int SupportHours { get; init; }
    public DateTime AssignmentDate { get; init; }
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}