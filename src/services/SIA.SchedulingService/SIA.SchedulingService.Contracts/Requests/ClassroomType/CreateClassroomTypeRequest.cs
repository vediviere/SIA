namespace SIA.SchedulingService.Contracts.Requests.ClassroomType;

public sealed class CreateClassroomTypeRequest
{
    public Guid TenantId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}