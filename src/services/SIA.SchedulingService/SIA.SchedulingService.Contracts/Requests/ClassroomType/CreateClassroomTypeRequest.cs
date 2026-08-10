namespace SIA.SchedulingService.Contracts.Requests.ClassroomType;

public sealed class CreateClassroomTypeRequest
{
    public Guid TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
}
