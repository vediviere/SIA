namespace SIA.AcademicStaffService.Application.DTOs.Persons;

public sealed record PersonDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string EmployeeNumber { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string PaternalLastName { get; init; } = default!;
    public string MaternalLastName { get; init; } = default!;
    public string AcademicDegree { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}