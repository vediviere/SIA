namespace SIA.SchoolControlService.Contracts.Responses.Students;

public sealed record StudentResponse
{
  public required Guid StudentId { get; init; }
  public required Guid TenantId { get; init; }
  public required string StudentNumber { get; init; }
  public required string FirstName { get; init; }
  public required string PaternalLastName { get; init; }
  public required string MaternalLastName { get; init; }
  public required bool Status { get; init; }
  public required DateTime CreatedAtUtc { get; init; }
  public DateTime? UpdatedAtUtc { get; init; }
}
