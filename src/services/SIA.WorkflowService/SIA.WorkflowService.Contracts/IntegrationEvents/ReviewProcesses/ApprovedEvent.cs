namespace SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;

public sealed record ApprovedEvent
{
  public required Guid EventId { get; init; }
  public required Guid CorrelationId { get; init; }
  public required DateTime OccurredAtUtc { get; init; }
  public required Guid TenantId { get; init; }
  public required Guid ReviewProcessId { get; init; }
  public required Guid ProposalId { get; init; }
  public required Guid DecidedBy { get; init; }
  public required int Version { get; init; }
}
