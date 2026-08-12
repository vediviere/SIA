namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;

public sealed record DivisionHeadCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid DivisionManagerId { get; init; }

    public required Guid ProgramId { get; init; }

    public required Guid PersonId { get; init; }

    public required string AcademicDegree { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}