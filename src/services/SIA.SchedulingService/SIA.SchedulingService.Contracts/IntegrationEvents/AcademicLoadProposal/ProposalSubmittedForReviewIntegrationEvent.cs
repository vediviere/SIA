using SIA.SchedulingService.Contracts.Enums;

namespace SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;

public sealed record ProposalSubmittedForReviewIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ProposalId { get; init; }
    public required Guid EducationalProgramId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required Guid DivisionHeadId { get; init; }
    public ProposalStatus ProposalStatus { get; init; } = ProposalStatus.SubmittedForReview;
    public required bool Status { get; init; }
    public int Version { get; init; } = 1;
}