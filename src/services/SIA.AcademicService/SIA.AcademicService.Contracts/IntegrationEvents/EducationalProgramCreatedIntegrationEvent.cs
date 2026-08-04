namespace SIA.AcademicService.Contracts.IntegrationEvents;

public sealed record EducationalProgramCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid EducationalProgramId { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string Level { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}