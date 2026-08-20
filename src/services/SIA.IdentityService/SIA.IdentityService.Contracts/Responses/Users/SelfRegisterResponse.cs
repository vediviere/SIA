namespace SIA.IdentityService.Contracts.Responses.Users;

public sealed record SelfRegisterResponse
{
  public required Guid Id { get; init; }

  public required Guid TenantId { get; init; }

  public required string Email { get; init; }

  public required bool MustChangePassword { get; init; }

  public required DateTime CreatedAtUtc { get; init; }

  public required Guid CorrelationId { get; init; }
}
