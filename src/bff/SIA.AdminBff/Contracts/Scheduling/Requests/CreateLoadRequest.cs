namespace SIA.AdminBff.Contracts.Scheduling.Requests;

public sealed record CreateLoadRequest
{
  public required Guid TeacherId { get; init; }

  public required Guid DivisionId { get; init; }

  public required Guid AcademicPeriodId { get; init; }

  public required string OfficialLetterNumber { get; init; }

  public required DateTime ProposedDate { get; init; }

  public required DateTime AssignmentDate { get; init; }
}
