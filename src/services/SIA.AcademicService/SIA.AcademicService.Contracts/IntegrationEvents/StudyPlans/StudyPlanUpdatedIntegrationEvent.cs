namespace SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;

public sealed record StudyPlanUpdatedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required Guid EducationalProgramId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Version { get; init; }
    public required DateOnly EffectiveFrom { get; init; }
    public required bool Status { get; init; }
    public int ContractVersion { get; init; } = 1;
}