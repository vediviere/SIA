using SIA.IdentityService.Infrastructure.Security;

namespace SIA.IdentityService.Tests.Infrastructure.Security;

public sealed class PasswordHasherTests
{
  [Fact]
  public void Hash_WithValidPassword_ShouldGenerateHash()
  {
    var passwordHasher = new PasswordHasherService();

    var hash = passwordHasher.Hash("Password123!");

    Assert.False(string.IsNullOrWhiteSpace(hash));
    Assert.NotEqual("Password123!", hash);
  }

  [Fact]
  public void Hash_SamePasswordTwice_ShouldGenerateDifferentHashes()
  {
    var passwordHasher = new PasswordHasherService();

    var firstHash = passwordHasher.Hash("Password123!");
    var secondHash = passwordHasher.Hash("Password123!");

    Assert.NotEqual(firstHash, secondHash);
  }

  [Fact]
  public void Verify_WithCorrectPassword_ShouldReturnTrue()
  {
    var passwordHasher = new PasswordHasherService();

    var hash = passwordHasher.Hash("Password123!");

    var result = passwordHasher.Verify(
        hash,
        "Password123!");

    Assert.True(result);
  }

  [Fact]
  public void Verify_WithIncorrectPassword_ShouldReturnFalse()
  {
    var passwordHasher = new PasswordHasherService();

    var hash = passwordHasher.Hash("Password123!");

    var result = passwordHasher.Verify(
        hash,
        "IncorrectPassword123!");

    Assert.False(result);
  }

  [Fact]
  public void Hash_WithEmptyPassword_ShouldThrowArgumentException()
  {
    var passwordHasher = new PasswordHasherService();

    Assert.Throws<ArgumentException>(() =>
        passwordHasher.Hash(""));
  }
}
