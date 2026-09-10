namespace SIA.AdminBff.Contracts.Scheduling.Requests;

public sealed record UpdateLoadRequest
{
  public required string OfficialLetterNumber { get; init; }

  public required DateTime ProposedDate { get; init; }

  public required DateTime AssignmentDate { get; init; }
}
