using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class AcademicLoadTests
{

  [Fact]
  public void Constructor_ValidData_CreateAcademicLoad()
  {
    var tenantId = Guid.NewGuid();
    var teacherId = Guid.NewGuid();
    var divisionId = Guid.NewGuid();
    var academicPeriodId = Guid.NewGuid();
    var proposedDate = new DateTime(2026, 1, 15);
    var assignmentDate = new DateTime(2026, 1, 20);

    var proposalId = Guid.NewGuid();

    var academicLoad = new AcademicLoad(tenantId, proposalId, teacherId, divisionId, academicPeriodId, "OF-2026-001", proposedDate, 10, 5, assignmentDate);

    Assert.NotEqual(Guid.Empty, academicLoad.Id);
    Assert.Equal(tenantId, academicLoad.TenantId);
    Assert.Equal(teacherId, academicLoad.TeacherId);
    Assert.Equal(divisionId, academicLoad.DivisionId);
    Assert.Equal(academicPeriodId, academicLoad.AcademicPeriodId);
    Assert.Equal("OF-2026-001", academicLoad.OfficialLetterNumber);
    Assert.Equal(proposedDate, academicLoad.ProposedDate);
    Assert.Equal(10, academicLoad.ClassHours);
    Assert.Equal(5, academicLoad.SupportHours);
    Assert.Equal(assignmentDate, academicLoad.AssignmentDate);
    Assert.True(academicLoad.Status);
    Assert.Null(academicLoad.UpdatedAtUtc);
    Assert.Equal(proposalId, academicLoad.ProposalId);
  }

  [Fact]
  public void Constructor_TrimOfficialLetterNumber()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
  }

  [Fact]
  public void Constructor_EmptyTenantId_ThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_EmptyTeacherId_ThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_EmptyDivisionId_ThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_EmptyAcademicPeriodId_ThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_EmptyOfficialLetterNumber_ThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_NegativeClassHours_ThrowArgumentOutOfRangeException()
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, -1, 5, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_NegativeSupportHours_ThrowArgumentOutOfRangeException()
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, -1, DateTime.UtcNow));
  }

  [Fact]
  public void Constructor_ZeroClassHoursAndSupportHours_DoesNotThrow()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
    Assert.Equal(0, academicLoad.ClassHours);
    Assert.Equal(0, academicLoad.SupportHours);
  }

  [Fact]
  public void Update_ValidData_UpdatesPropertiesAndSetsUpdatedAtUtc()
  {
    var tenantId = Guid.NewGuid();
    var proposalId = Guid.NewGuid();
    var teacherId = Guid.NewGuid();
    var divisionId = Guid.NewGuid();
    var academicPeriodId = Guid.NewGuid();
    var newProposedDate = new DateTime(2026, 2, 1);
    var newAssignmentDate = new DateTime(2026, 2, 5);

    var academicLoad = new AcademicLoad(tenantId, proposalId, teacherId, divisionId, academicPeriodId, "OF-2026-001", newProposedDate, 10, 5, newAssignmentDate);

    academicLoad.Update("OF-2026-002", newProposedDate, newAssignmentDate);

    Assert.Equal(tenantId, academicLoad.TenantId);
    Assert.Equal(proposalId, academicLoad.ProposalId);
    Assert.Equal(teacherId, academicLoad.TeacherId);
    Assert.Equal(divisionId, academicLoad.DivisionId);
    Assert.Equal(academicPeriodId, academicLoad.AcademicPeriodId);
    Assert.Equal("OF-2026-002", academicLoad.OfficialLetterNumber);
    Assert.Equal(newProposedDate, academicLoad.ProposedDate);
    Assert.Equal(10, academicLoad.ClassHours);
    Assert.Equal(5, academicLoad.SupportHours);
    Assert.Equal(newAssignmentDate, academicLoad.AssignmentDate);
    Assert.NotNull(academicLoad.UpdatedAtUtc);
  }

  [Fact]
  public void Update_EmptyOfficialLetterNumber_ThrowArgumentException()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
    Assert.Throws<ArgumentException>(() => academicLoad.Update("", DateTime.UtcNow, DateTime.UtcNow));
  }

  [Fact]
  public void SetClassHours_WithValidValue_ShouldUpdateTotal()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);

    academicLoad.SetClassHours(20);

    Assert.Equal(20, academicLoad.ClassHours);
  }

  [Fact]
  public void SetClassHours_WithNegativeValue_ShouldThrow()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);

    Assert.Throws<ArgumentOutOfRangeException>(
        () => academicLoad.SetClassHours(-1));
  }

  [Fact]
  public void SetSupportHours_WithValidValue_ShouldUpdateTotal()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);

    academicLoad.SetSupportHours(8);

    Assert.Equal(8, academicLoad.SupportHours);
    Assert.NotNull(academicLoad.UpdatedAtUtc);
  }

  [Fact]
  public void SetSupportHours_WithNegativeValue_ShouldThrow()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);

    Assert.Throws<ArgumentOutOfRangeException>(() => academicLoad.SetSupportHours(-1));
  }

  [Fact]
  public void Deactivate_StatusFalse()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
    academicLoad.Deactivate();

    Assert.False(academicLoad.Status);
    Assert.NotNull(academicLoad.UpdatedAtUtc);
  }

  [Fact]
  public void Activate_StatusTrue()
  {
    var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
    academicLoad.Activate();

    Assert.True(academicLoad.Status);
    Assert.NotNull(academicLoad.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_EmptyProposalId_ShouldThrowArgumentException()
  {
    Assert.Throws<ArgumentException>(() =>
      new AcademicLoad(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
  }


}
