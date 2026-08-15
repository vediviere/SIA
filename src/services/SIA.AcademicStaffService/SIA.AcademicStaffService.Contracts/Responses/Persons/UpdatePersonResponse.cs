namespace SIA.AcademicStaffService.Contracts.Responses.Persons;

public sealed record UpdatePersonResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string EmployeeNumber { get; init; }

    public required string FirstName { get; init; }

    public required string PaternalLastName { get; init; }

    public required string MaternalLastName { get; init; }

    public required string AcademicDegree { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }

    public required bool Status { get; init; }

    public required DateTime? UpdatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}