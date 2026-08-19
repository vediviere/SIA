namespace SIA.IdentityService.Contracts.Requests.Users;

public sealed record SelfRegisterRequest
{
  public required string InstitutionCode { get; init; }

  public required string Email { get; init; }

  public required string Password { get; init; }
}
