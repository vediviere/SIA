using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Domain;

public sealed class RefreshTokenTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateActiveToken()
  {
    var token = new RefreshToken(Guid.NewGuid(), "token-hash", DateTime.UtcNow.AddDays(7));

    Assert.NotEqual(Guid.Empty, token.Id);
    Assert.True(token.IsActive(DateTime.UtcNow));
    Assert.Null(token.RevokedAtUtc);
  }

  [Fact]
  public void Revoke_ShouldDeactivateToken()
  {
    var token = new RefreshToken(Guid.NewGuid(), "token-hash", DateTime.UtcNow.AddDays(7));

    token.Revoke();

    Assert.NotNull(token.RevokedAtUtc);
    Assert.False(token.IsActive(DateTime.UtcNow));
  }
}
