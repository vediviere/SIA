namespace SIA.TenancyService.Contracts.Requests.Tenants;

public sealed record ResolveTenantRequest
{
  public required string InstituteCode { get; init; }
  public required string Email { get; init; }
}
