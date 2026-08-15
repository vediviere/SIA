namespace SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;

public sealed record CreateTeachingSupportHoursRequest
{
    public required Guid TenantId { get; init; }
    public required Guid ActivityId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required int Hours { get; init; }
}