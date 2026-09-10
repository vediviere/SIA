namespace SIA.AdminBff.Clients.Scheduling;

public sealed record LoadUpdateDto
{
  public required string OfficialLetterNumber { get; init; }

  public required DateTime ProposedDate { get; init; }

  public required DateTime AssignmentDate { get; init; }
}
