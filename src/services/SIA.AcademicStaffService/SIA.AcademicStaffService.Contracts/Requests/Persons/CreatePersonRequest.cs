namespace SIA.AcademicStaffService.Contracts.Requests.Persons;

public sealed record CreatePersonRequest
{
    public required Guid TenantId { get; init; }
    public required string EmployeeNumber { get; init; }

    public required string FirstName { get; init; }

    public required string PaternalLastName { get; init; }

    public required string MaternalLastName { get; init; }

    public required string AcademicDegree { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }
}