
namespace SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

public sealed record ClassroomLabCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ClassroomLabId { get; init; }
    public required Guid BuildingId { get; init; }
    public required Guid ClassroomTypeId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Capacity { get; init; }
    public required bool Status { get; init; }
    public required int Version { get; init; }
}