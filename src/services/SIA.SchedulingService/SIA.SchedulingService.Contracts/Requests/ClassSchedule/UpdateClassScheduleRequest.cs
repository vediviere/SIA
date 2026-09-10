namespace SIA.SchedulingService.Contracts.Requests.ClassSchedule;

public sealed record UpdateClassScheduleRequest
{
    public required string Day { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
}