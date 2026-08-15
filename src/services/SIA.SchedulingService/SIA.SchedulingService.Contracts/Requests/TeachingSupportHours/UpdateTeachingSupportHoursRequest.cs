
namespace SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;

public sealed record UpdateTeachingSupportHoursRequest
{
    public required int Hours {  get; set; }
}