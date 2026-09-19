using SIA.WorkflowService.Contracts.Enums;

namespace SIA.WorkflowService.Contracts.Requests.ReviewProcesses;

public sealed record ObservationRequest
{
  public required ObservationTarget TargetType { get; init; }
  public required Guid TargetId { get; init; }
  public required string Description { get; init; }
}
