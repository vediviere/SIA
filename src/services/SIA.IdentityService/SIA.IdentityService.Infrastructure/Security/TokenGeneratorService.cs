using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.Models;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Infrastructure.Security;

public sealed class TokenGeneratorService : ITokenGenerator
{
  private readonly TokenSettings _settings;

  public TokenGeneratorService(TokenSettings settings)
  {
    _settings = settings;
  }

  public TokenResult Generate(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions)
  {
    var now = DateTime.UtcNow;
    var expiresAtUtc = now.AddMinutes(_settings.ExpirationMinutes);

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new(JwtRegisteredClaimNames.Email, user.Email),
      new("tenant_id", user.TenantId.ToString())
    };

    claims.AddRange(roles.Select(role => new Claim("role", role)));
    claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

    var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(_settings.SigningKey));

    var descriptor = new SecurityTokenDescriptor
    {
      Issuer = _settings.Issuer,
      Audience = _settings.Audience,
      Subject = new ClaimsIdentity(claims),
      IssuedAt = now,
      NotBefore = now,
      Expires = expiresAtUtc,
      SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
    };

    var token = new JsonWebTokenHandler().CreateToken(descriptor);

    return new TokenResult(token, expiresAtUtc);
  }
}
