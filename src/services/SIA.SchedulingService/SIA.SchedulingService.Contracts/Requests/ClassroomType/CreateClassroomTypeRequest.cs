namespace SIA.SchedulingService.Contracts.Requests.ClassroomType;

public sealed class CreateClassroomTypeRequest
{
    public required Guid TenantId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}