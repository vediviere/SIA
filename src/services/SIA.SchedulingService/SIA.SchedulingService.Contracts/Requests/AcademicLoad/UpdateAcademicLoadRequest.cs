
namespace SIA.SchedulingService.Contracts.Requests.AcademicLoad;

public sealed record UpdateAcademicLoadRequest
{
    public required string OfficialLetterNumber { get; init; }
    public required DateTime ProposedDate { get; init; }
    public required DateTime AssignmentDate { get; init; }
}
