namespace SIA.SchoolControlService.Contracts.Requests.Students;

public sealed record LinkRequest
{
  public required Guid TenantId { get; init; }
  public required Guid UserId { get; init; }
  public required string StudentNumber { get; init; }
}
