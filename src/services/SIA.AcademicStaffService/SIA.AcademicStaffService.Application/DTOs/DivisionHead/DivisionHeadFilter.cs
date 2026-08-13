namespace SIA.AcademicStaffService.Application.DTOs.DivisionManagers;

public sealed class DivisionHeadFilter
{
    public Guid TenantId { get; init; }

    public Guid? ProgramId { get; init; }

    public Guid? PersonId { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
