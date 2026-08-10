namespace SIA.SchedulingService.Contracts.Requests.Classroom;

public sealed class UpdateClassroomRequest
{
    public Guid BuildingId { get; init; }
    public Guid ClassroomTypeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public string Description { get; init; } = string.Empty;
}