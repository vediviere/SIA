using SIA.SchedulingService.Contracts.Enums;

namespace SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;

public sealed record SubmitProposalForReviewResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid EducationalProgramId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required Guid DivisionHeadId { get; init; }
    public required ProposalStatus ProposalStatus { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime? UpdatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}