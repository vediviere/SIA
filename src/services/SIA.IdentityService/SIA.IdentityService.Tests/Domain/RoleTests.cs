using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Domain;

public sealed class RoleTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateRole()
  {
    var role = new Role("Teacher", "Docente");

    Assert.NotEqual(Guid.Empty, role.Id);
    Assert.Equal("Teacher", role.Code);
    Assert.Equal("Docente", role.Description);
    Assert.NotEqual(default, role.CreatedAtUtc);
    Assert.Null(role.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_ShouldTrimCodeAndDescription()
  {
    var role = new Role("  Teacher  ", "  Docente  ");

    Assert.Equal("Teacher", role.Code);
    Assert.Equal("Docente", role.Description);
  }

  [Fact]
  public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new Role("", "Docente"));
  }

  [Fact]
  public void Constructor_WithEmptyDescription_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new Role("Teacher", ""));
  }
}
