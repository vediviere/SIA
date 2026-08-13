namespace SIA.AcademicStaffService.Application.DTOs.Coordinators;

public sealed class CoordinatorFilter
{
    public Guid TenantId { get; init; }

    public Guid? PersonId { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}