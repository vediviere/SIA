namespace SIA.AcademicStaffService.Application.DTOs.DivisionHeads;

public sealed record DivisionHeadDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid ProgramId { get; init; }
    public Guid PersonId { get; init; }
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}