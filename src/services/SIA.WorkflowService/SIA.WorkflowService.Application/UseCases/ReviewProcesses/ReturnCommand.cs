namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed record ReturnCommand
{
  public required Guid ProcessId { get; init; }
  public required Guid TenantId { get; init; }
  public required Guid DecidedBy { get; init; }
  public required Guid CorrelationId { get; init; }
  public required IReadOnlyCollection<ObservationInput> Observations { get; init; }
}
