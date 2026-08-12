namespace SIA.SchedulingService.Contracts.Responses.Classrooms;

public sealed class UpdateClassroomLabResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid BuildingId { get; init; }
    public Guid ClassroomTypeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool Status { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }
}