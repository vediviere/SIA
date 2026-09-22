namespace SIA.WorkflowService.Contracts.Responses.ReviewProcesses;

public sealed class ReviewProcessListItemResponse
{
    public Guid Id { get; init; }
    public Guid AcademicLoadProposalId { get; init; }
    public Guid DivisionHeadId { get; init; }
    public int Version { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}