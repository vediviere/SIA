namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;

public sealed record TeacherActivatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid TeacherId { get; init; }
    public int Version { get; init; } = 1;
}