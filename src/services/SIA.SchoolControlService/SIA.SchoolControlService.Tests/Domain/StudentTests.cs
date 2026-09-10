using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Tests.Domain;

public sealed class StudentTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldCreateActiveStudent()
  {
    var tenantId = Guid.NewGuid();

    var student = new Student(
      tenantId,
      " 2026-ab-001 ",
      " Marco Antonio ",
      " Morales ",
      " Castillo ");

    Assert.NotEqual(Guid.Empty, student.Id);
    Assert.Equal(tenantId, student.TenantId);
    Assert.Equal("2026-AB-001", student.StudentNumber);
    Assert.Equal("Marco Antonio", student.FirstName);
    Assert.Equal("Morales", student.PaternalLastName);
    Assert.Equal("Castillo", student.MaternalLastName);
    Assert.True(student.Status);
    Assert.NotEqual(default, student.CreatedAtUtc);
    Assert.Null(student.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.Empty, "20260001", "Marco", "Morales", "Castillo"));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithInvalidStudentNumber_ShouldThrowArgumentException(string studentNumber)
  {
    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.NewGuid(), studentNumber, "Marco", "Morales", "Castillo"));
  }

  [Fact]
  public void Constructor_WithStudentNumberTooLong_ShouldThrowArgumentException()
  {
    var studentNumber = new string('A', Student.StudentNumberMaxLength + 1);

    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.NewGuid(), studentNumber, "Marco", "Morales", "Castillo"));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithInvalidFirstName_ShouldThrowArgumentException(string firstName)
  {
    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.NewGuid(), "20260001", firstName, "Morales", "Castillo"));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithInvalidPaternalLastName_ShouldThrowArgumentException(string paternalLastName)
  {
    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.NewGuid(), "20260001", "Marco", paternalLastName, "Castillo"));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithInvalidMaternalLastName_ShouldThrowArgumentException(string maternalLastName)
  {
    Assert.Throws<ArgumentException>(() =>
      new Student(Guid.NewGuid(), "20260001", "Marco", "Morales", maternalLastName));
  }
}
