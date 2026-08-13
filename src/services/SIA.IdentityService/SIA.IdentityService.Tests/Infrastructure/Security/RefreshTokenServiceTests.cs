using SIA.IdentityService.Infrastructure.Security;

namespace SIA.IdentityService.Tests.Infrastructure.Security;

public sealed class RefreshTokenServiceTests
{
  [Fact]
  public void Generate_ShouldCreateTokenAndHash()
  {
    var service = CreateService();

    var result = service.Generate();

    Assert.False(string.IsNullOrWhiteSpace(result.Token));
    Assert.False(string.IsNullOrWhiteSpace(result.TokenHash));
    Assert.NotEqual(result.Token, result.TokenHash);
    Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);
  }

  [Fact]
  public void Hash_WithSameToken_ShouldReturnSameHash()
  {
    var service = CreateService();

    var first = service.Hash("refresh-token");
    var second = service.Hash("refresh-token");

    Assert.Equal(first, second);
  }

  private static RefreshTokenService CreateService()
  {
    return new RefreshTokenService(new TokenSettings
    {
      Issuer = "SIA.IdentityService",
      Audience = "SIA.Platform",
      SigningKey = "AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyA=",
      ExpirationMinutes = 5,
      RefreshTokenExpirationDays = 7
    });
  }
}
