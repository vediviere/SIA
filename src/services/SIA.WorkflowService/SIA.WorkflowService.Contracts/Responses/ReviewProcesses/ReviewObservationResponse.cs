using SIA.WorkflowService.Contracts.Enums;

namespace SIA.WorkflowService.Contracts.Responses.ReviewProcesses;

public sealed class ReviewObservationResponse
{
    public Guid Id { get; init; }
    public ObservationTarget TargetType { get; init; }
    public Guid TargetId { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}