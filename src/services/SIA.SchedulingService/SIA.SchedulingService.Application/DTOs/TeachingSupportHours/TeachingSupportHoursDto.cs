namespace SIA.SchedulingService.Application.DTOs.TeachingSupportHours;

public sealed class TeachingSupportHoursDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid ActivityId { get; init; }
    public Guid AcademicLoadId { get; init; }
    public int Hours { get; init; }
    public bool Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}