using SIA.IdentityService.Domain.Entities;

namespace SIA.IdentityService.Tests.Domain;

public sealed class PermissionTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreatePermission()
  {
    var permission = new Permission("Grades.Read", "Consultar calificaciones");

    Assert.NotEqual(Guid.Empty, permission.Id);
    Assert.Equal("Grades.Read", permission.Code);
    Assert.Equal("Consultar calificaciones", permission.Description);
    Assert.NotEqual(default, permission.CreatedAtUtc);
    Assert.Null(permission.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_ShouldTrimCodeAndDescription()
  {
    var permission = new Permission("  Grades.Read  ", "  Consultar calificaciones  ");

    Assert.Equal("Grades.Read", permission.Code);
    Assert.Equal("Consultar calificaciones", permission.Description);
  }

  [Fact]
  public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new Permission("", "Consultar calificaciones"));
  }

  [Fact]
  public void Constructor_WithEmptyDescription_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
        new Permission("Grades.Read", ""));
  }
}
