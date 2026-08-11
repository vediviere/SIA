namespace SIA.AcademicStaffService.Application.DTOs.Professors;

public sealed class ProfessorFilter
{
    public Guid TenantId { get; init; }

    public Guid? PersonId { get; init; }

    public string? ContractType { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}