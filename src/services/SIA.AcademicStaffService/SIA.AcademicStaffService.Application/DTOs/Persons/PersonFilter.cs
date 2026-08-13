namespace SIA.AcademicStaffService.Application.DTOs.Persons;

public sealed class PersonFilter
{
    public Guid TenantId { get; init; }

    public string? EmployeeNumber { get; init; }

    public string? FirstName { get; init; }

    public string? PaternalLastName { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}