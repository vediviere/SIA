using Microsoft.IdentityModel.JsonWebTokens;
using SIA.IdentityService.Infrastructure.Security;
using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Infrastructure.Security;

public sealed class TokenGeneratorTests
{
  private const string SigningKey = "AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyA=";

  [Fact]
  public void Generate_WithUserAndRoles_ShouldCreateToken()
  {
    var user = new User(Guid.NewGuid(), "admin@institucion.edu.mx", "hash");
    var generator = CreateGenerator();

    var result = generator.Generate(user, ["Administrator", "Teacher"], ["Users.Manage"]);
    var jwt = new JsonWebToken(result.Token);

    Assert.False(string.IsNullOrWhiteSpace(result.Token));
    Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == user.Id.ToString());
    Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == user.Email);
    Assert.Contains(jwt.Claims, claim => claim.Type == "tenant_id" && claim.Value == user.TenantId.ToString());
    Assert.Equal(2, jwt.Claims.Count(claim => claim.Type == "role"));
    Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);
    Assert.Contains(jwt.Claims, claim => claim.Type == "permission" && claim.Value == "Users.Manage");
  }

  [Fact]
  public void Generate_WithoutRoles_ShouldCreateTokenWithoutRoleClaims()
  {
    var user = new User(Guid.NewGuid(), "user@institucion.edu.mx", "hash");
    var result = CreateGenerator().Generate(user, [], []);
    var jwt = new JsonWebToken(result.Token);

    Assert.DoesNotContain(jwt.Claims, claim => claim.Type == "role");
    Assert.DoesNotContain(jwt.Claims, claim => claim.Type == "permission");
  }

  private static TokenGeneratorService CreateGenerator()
  {
    return new TokenGeneratorService(new TokenSettings
    {
      Issuer = "SIA.IdentityService",
      Audience = "SIA.Platform",
      SigningKey = SigningKey,
      ExpirationMinutes = 60,
      RefreshTokenExpirationDays = 7
    });
  }
}
