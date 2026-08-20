namespace SIA.TenancyService.Contracts.Responses.Tenants;

public sealed record ResolveTenantResponse
{
  public required Guid TenantId { get; init; }
  public required string InstituteCode { get; init; }
}
