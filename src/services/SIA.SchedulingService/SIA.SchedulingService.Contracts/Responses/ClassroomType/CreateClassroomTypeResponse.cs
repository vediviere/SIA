namespace SIA.SchedulingService.Contracts.Responses.ClassroomType;

public sealed class CreateClassroomTypeResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}
