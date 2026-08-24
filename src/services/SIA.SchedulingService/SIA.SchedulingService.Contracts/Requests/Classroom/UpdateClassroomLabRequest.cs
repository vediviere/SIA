namespace SIA.SchedulingService.Contracts.Requests.Classroom;

public sealed record UpdateClassroomLabRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Capacity { get; init; }
    public required string Description { get; init; }
}