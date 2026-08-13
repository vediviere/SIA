

namespace SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;

public sealed class ClassroomTypeCreatedIntegrationEvent
{
    public Guid EventId { get; init; }
    public Guid CorrelationId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public Guid TenantId { get; init; }
    public Guid ClassroomTypeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Status { get; init; }
    public int Version { get; init; }
}