namespace SIA.WorkflowService.Contracts.Requests.ReviewProcesses;

public sealed record ReturnRequest
{
  public required IReadOnlyCollection<ObservationRequest> Observations { get; init; }
}
