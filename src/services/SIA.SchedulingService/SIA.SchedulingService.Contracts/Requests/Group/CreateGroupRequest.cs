namespace SIA.SchedulingService.Contracts.Requests.Group;

public sealed record CreateGroupRequest
{
    public required Guid EducationalProgramId { get; init; }
    public required string GroupName { get; init; }
    public required string Shift { get; init; }
    public required int Capacity { get; init; }
}