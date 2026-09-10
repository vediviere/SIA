namespace SIA.SchedulingService.Contracts.Requests.SupportSchedules;

public sealed record UpdateSupportScheduleRequest
{
    public required string Day { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
}