namespace SIA.SchoolControlService.Contracts.Requests.Students;

public sealed record CreateRequest
{
  public required string StudentNumber { get; init; }
  public required string FirstName { get; init; }
  public required string PaternalLastName { get; init; }
  public required string MaternalLastName { get; init; }
}
