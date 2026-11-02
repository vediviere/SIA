namespace SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;

public sealed record AssignTutorRequest
{
    public Guid GroupId { get; init; }
    public Guid AcademicLoadId { get; init; }
}