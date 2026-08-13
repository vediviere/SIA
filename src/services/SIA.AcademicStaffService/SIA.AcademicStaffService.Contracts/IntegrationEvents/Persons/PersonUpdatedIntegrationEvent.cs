namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;

public sealed record PersonUpdatedIntegrationEvent
{
    public required Guid EventId { get; init; }

    public required Guid CorrelationId { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public required string EmployeeNumber { get; init; }

    public required string FirstName { get; init; }

    public required string PaternalLastName { get; init; }

    public required string MaternalLastName { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }

    public required bool Status { get; init; }

    public int Version { get; init; } = 1;
}