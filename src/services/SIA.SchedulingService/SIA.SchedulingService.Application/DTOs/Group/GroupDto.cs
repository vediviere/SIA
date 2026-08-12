namespace SIA.SchedulingService.Application.DTOs.Group;

public sealed record GroupDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid EducationalProgramId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public string Shift {  get; init; } = string.Empty;
    public int Capacity {  get; init; } 
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
