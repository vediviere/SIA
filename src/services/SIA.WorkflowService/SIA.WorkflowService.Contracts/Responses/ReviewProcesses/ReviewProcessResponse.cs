namespace SIA.WorkflowService.Contracts.Responses.ReviewProcesses;

public sealed class ReviewProcessResponse
{
    public Guid Id { get; init; }
    public Guid AcademicLoadProposalId { get; init; }
    public Guid DivisionHeadId { get; init; }
    public int Version { get; init; }
    public int Status { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid CorrelationId { get; init; }

    public IReadOnlyList<ReviewObservationResponse> Observations { get; init; } = [];
}