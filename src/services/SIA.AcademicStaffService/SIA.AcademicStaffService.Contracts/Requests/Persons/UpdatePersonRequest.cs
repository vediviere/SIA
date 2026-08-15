namespace SIA.AcademicStaffService.Contracts.Requests.Persons;

public sealed record UpdatePersonRequest
{
    public required string FirstName { get; init; }

    public required string PaternalLastName { get; init; }

    public required string MaternalLastName { get; init; }

    public required string AcademicDegree { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }
}