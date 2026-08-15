namespace SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;

public sealed record CreateTeachingSupportHoursResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ActivityId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required int Hours { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}