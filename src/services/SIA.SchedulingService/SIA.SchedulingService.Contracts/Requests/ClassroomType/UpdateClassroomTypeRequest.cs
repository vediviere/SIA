namespace SIA.SchedulingService.Contracts.Requests.ClassroomType;

public sealed record UpdateClassroomTypeRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}