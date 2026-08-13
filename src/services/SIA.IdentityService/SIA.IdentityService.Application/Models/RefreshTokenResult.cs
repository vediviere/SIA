namespace SIA.IdentityService.Application.Models;

public sealed record RefreshTokenResult(string Token, string TokenHash, DateTime ExpiresAtUtc);
