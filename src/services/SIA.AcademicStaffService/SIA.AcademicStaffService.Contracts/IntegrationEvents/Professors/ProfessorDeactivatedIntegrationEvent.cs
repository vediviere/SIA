namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;

public sealed record ProfessorDeactivatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProfessorId { get; init; }

    public int Version { get; init; } = 1;
}