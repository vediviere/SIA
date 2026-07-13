namespace SIA.SchoolControlService.Contracts.Responses;

public sealed record SubjectReferenceResponse
{
  public required Guid SubjectId { get; init; }

  public required Guid TenantId { get; init; }

  public required string Code { get; init; }

  public required string Name { get; init; }

  public required int Credits { get; init; }

  public required string Status { get; init; }

  public required DateTime UpdatedAtUtc { get; init; }
}
