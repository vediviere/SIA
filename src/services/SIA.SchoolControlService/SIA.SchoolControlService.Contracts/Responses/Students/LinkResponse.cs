namespace SIA.SchoolControlService.Contracts.Responses.Students;

public sealed record LinkResponse
{
  public required Guid StudentId { get; init; }
  public required Guid UserId { get; init; }
  public required string StudentNumber { get; init; }
  public required bool Linked { get; init; }
}
