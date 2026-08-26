namespace SIA.SchedulingService.Contracts.Requests.ClassSchedule;

public sealed record CreateClassScheduleRequest
{
    public required Guid TenantId { get; init; }
    public required Guid OfferingId { get; init; }
    public required Guid ClassroomLabId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required string Day { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
}