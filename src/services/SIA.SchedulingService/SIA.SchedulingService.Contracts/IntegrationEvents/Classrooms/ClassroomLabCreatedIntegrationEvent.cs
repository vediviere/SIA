
namespace SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

public sealed class ClassroomLabCreatedIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid ClassroomLabId { get; init; }
    public Guid BuildingId { get; init; }
    public Guid ClassroomTypeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public bool Status { get; init; }
    public int Version { get; init; }
}
