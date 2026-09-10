namespace SIA.SchedulingService.Contracts.Responses.ClassSchedule;

public sealed record CreateClassScheduleResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid OfferingId { get; init; }
    public required Guid ClassroomLabId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required string Day { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}