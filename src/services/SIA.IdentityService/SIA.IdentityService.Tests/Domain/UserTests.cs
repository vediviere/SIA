using SIA.IdentityService.Domain.Entities;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Tests.Domain;

public sealed class UserTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateActiveUser()
  {
    var tenantId = Guid.NewGuid();

    var user = new User(
        tenantId,
        " USER@INSTITUTION.EDU.MX ",
        "password-hash");

    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.Equal(tenantId, user.TenantId);
    Assert.Equal("user@institution.edu.mx", user.Email);
    Assert.Equal("password-hash", user.PasswordHash);
    Assert.Equal(UserStatus.Active, user.Status);
    Assert.False(user.MustChangePassword);
  }

  [Fact]
  public void Constructor_WithMustChangePassword_ShouldSetFlag()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "password-hash", mustChangePassword: true);

    Assert.True(user.MustChangePassword);
  }

  [Fact]
  public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new User(Guid.Empty, "user@institution.edu.mx", "password-hash"));
  }

  [Fact]
  public void Constructor_WithEmptyEmail_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), "", "password-hash"));
  }

  [Fact]
  public void Constructor_WithEmptyPasswordHash_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), "user@institution.edu.mx", ""));
  }

  [Fact]
  public void Lock_ShouldChangeStatusToLocked()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "password-hash");

    user.Lock();

    Assert.Equal(UserStatus.Locked, user.Status);
    Assert.NotNull(user.UpdatedAtUtc);
  }

  [Fact]
  public void Deactivate_ShouldChangeStatusToInactive()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "password-hash");

    user.Deactivate();

    Assert.Equal(UserStatus.Inactive, user.Status);
    Assert.NotNull(user.UpdatedAtUtc);
  }

  [Fact]
  public void Activate_ShouldChangeStatusToActive()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "password-hash");

    user.Lock();

    user.Activate();

    Assert.Equal(UserStatus.Active, user.Status);
    Assert.NotNull(user.UpdatedAtUtc);
  }

  [Fact]
  public void ChangePassword_WithValidHash_ShouldUpdatePassword()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "old-hash", mustChangePassword: true);

    user.ChangePassword("new-hash");

    Assert.Equal("new-hash", user.PasswordHash);
    Assert.False(user.MustChangePassword);
    Assert.NotNull(user.UpdatedAtUtc);
  }

  [Fact]
  public void ChangePassword_WithEmptyHash_ShouldThrowArgumentException()
  {
    var user = new User(Guid.NewGuid(), "user@institution.edu.mx", "old-hash", mustChangePassword: true);

    Assert.Throws<ArgumentException>(() => user.ChangePassword(""));
  }
}
