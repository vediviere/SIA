using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed record ObservationInput
{
  public required ObservationTarget TargetType { get; init; }
  public required Guid TargetId { get; init; }
  public required string Description { get; init; }
}
