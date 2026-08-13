using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Domain;

public sealed class UserRoleTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateActiveUserRole()
  {
    var userId = Guid.NewGuid();
    var roleId = Guid.NewGuid();  

    var userRole = new UserRole(userId, roleId);

    Assert.NotEqual(Guid.Empty, userRole.Id);
    Assert.Equal(userId, userRole.UserId);
    Assert.Equal(roleId, userRole.RoleId);
    Assert.NotEqual(default, userRole.CreatedAtUtc);
    Assert.Null(userRole.RevokedAtUtc);
    Assert.True(userRole.IsActive);
  }

  [Fact]
  public void Constructor_WithEmptyUserId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new UserRole(Guid.Empty, Guid.NewGuid()));
  }

  [Fact]
  public void Constructor_WithEmptyRoleId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new UserRole(Guid.NewGuid(), Guid.Empty));
  }

  [Fact]
  public void Revoke_ShouldDeactivateUserRole()
  {
    var userRole = new UserRole(Guid.NewGuid(), Guid.NewGuid());

    userRole.Revoke();

    Assert.False(userRole.IsActive);
    Assert.NotNull(userRole.RevokedAtUtc);
  }

  [Fact]
  public void Revoke_WhenAlreadyRevoked_ShouldKeepOriginalRevokedDate()
  {
    var userRole = new UserRole(Guid.NewGuid(), Guid.NewGuid());

    userRole.Revoke();

    var originalRevokedAtUtc = userRole.RevokedAtUtc;

    userRole.Revoke();

    Assert.Equal(originalRevokedAtUtc, userRole.RevokedAtUtc);
  }
}
