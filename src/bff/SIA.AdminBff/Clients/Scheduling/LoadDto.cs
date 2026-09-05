namespace SIA.AdminBff.Clients.Scheduling;

public sealed record LoadDto
{
  public required Guid Id { get; init; }

  public required Guid TenantId { get; init; }

  public required Guid ProposalId { get; init; }

  public required Guid TeacherId { get; init; }

  public required Guid DivisionId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required string OfficialLetterNumber { get; init; }

  public required DateTime ProposedDate { get; init; }

  public required int ClassHours { get; init; }

  public required int SupportHours { get; init; }

  public required DateTime AssignmentDate { get; init; }

  public required bool Status { get; init; }

  public required DateTime CreatedAtUtc { get; init; }

  public DateTime? UpdatedAtUtc { get; init; }

  public required Guid CorrelationId { get; init; }
}
