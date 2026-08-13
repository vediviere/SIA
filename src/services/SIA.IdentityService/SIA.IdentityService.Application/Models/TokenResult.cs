namespace SIA.IdentityService.Application.Models;

public sealed record TokenResult(string Token, DateTime ExpiresAtUtc);
