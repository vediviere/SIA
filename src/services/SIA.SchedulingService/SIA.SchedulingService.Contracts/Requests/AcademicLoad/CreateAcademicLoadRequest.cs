namespace SIA.SchedulingService.Contracts.Requests.AcademicLoad;

public sealed record CreateAcademicLoadRequest
{
    public required Guid TenantId { get; init; }
    public required Guid TeacherId { get; init; }
    public required Guid DivisionId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required string OfficialLetterNumber { get; init; }
    public required DateTime ProposedDate { get; init; }
    public required int ClassHours { get; init; }
    public required int SupportHours { get; init; }
    public required DateTime AssignmentDate { get; init; }
}