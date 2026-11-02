namespace SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;

public sealed record RemoveTutorRequest
{
    public required Guid GroupId { get; init; }
}
