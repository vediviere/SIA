namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;

public sealed record CoordinatorUpdatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid CoordinatorId { get; init; }

    public required Guid PersonId { get; init; }

    public required string AcademicDegree { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}