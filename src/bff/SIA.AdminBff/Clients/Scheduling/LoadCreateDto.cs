namespace SIA.AdminBff.Clients.Scheduling;

public sealed record LoadCreateDto
{
  public required Guid TenantId { get; init; }

  public required Guid ProposalId { get; init; }

  public required Guid TeacherId { get; init; }

  public required Guid DivisionId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required string OfficialLetterNumber { get; init; }

  public required DateTime ProposedDate { get; init; }

  public required DateTime AssignmentDate { get; init; }
}
