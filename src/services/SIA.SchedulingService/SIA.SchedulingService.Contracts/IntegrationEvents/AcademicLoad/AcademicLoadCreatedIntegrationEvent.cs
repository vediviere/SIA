namespace SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;

public sealed record AcademicLoadCreatedIntegrationEvent
{
  public required Guid EventId { get; init; }
  public required Guid CorrelationId { get; init; }
  public required DateTime OccurredAtUtc { get; init; }
  public required Guid TenantId { get; init; }
  public required Guid ProposalId { get; init; }
  public required Guid AcademicLoadId { get; init; }
  public required Guid TeacherId { get; init; }
  public required Guid DivisionId { get; init; }
  public required Guid AcademicPeriodId { get; init; }
  public required string OfficialLetterNumber { get; init; }
  public required DateTime ProposedDate { get; init; }
  public required DateTime AssignmentDate { get; init; }
  public required int ClassHours { get; init; }
  public required int SupportHours { get; init; }
  public required bool Status { get; init; }
  public int Version { get; init; } = 1;
}
