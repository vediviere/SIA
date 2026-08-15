namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;

public sealed record TeacherCreatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProfessorId { get; init; }

    public required Guid PersonId { get; init; }

    public required string ProfessionalProfile { get; init; }

    public required string ContractType { get; init; }

    public required int ContractHours { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}