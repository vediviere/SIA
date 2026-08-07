namespace SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;

public sealed record StudyPlanRestoredIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public int ContractVersion { get; init; } = 1;
}