namespace SIA.SchedulingService.Contracts.Responses.ClassroomType;

public sealed class UpdateClassroomTypeResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}
