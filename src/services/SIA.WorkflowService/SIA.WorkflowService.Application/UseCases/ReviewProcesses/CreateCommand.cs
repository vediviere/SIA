namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed record CreateCommand
{
  public required Guid EventId { get; init; }
  public required string EventType { get; init; }
  public required string SourceService { get; init; }
  public required Guid TenantId { get; init; }
  public required Guid AcademicLoadProposalId { get; init; }
  public required Guid DivisionHeadId { get; init; }
  public required int Version { get; init; }
  public required DateTime SubmittedAtUtc { get; init; }
  public required Guid CorrelationId { get; init; }
}
