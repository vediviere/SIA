namespace SIA.AcademicService.Contracts.Requests;

public sealed record CreateSubjectRequest
{
  public required Guid TenantId { get; init; }

  public required string Code { get; init; }

  public required string Name { get; init; }

  public required int Credits { get; init; }
}
