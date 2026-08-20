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

        var academicLoad = new AcademicLoad(tenantId, teacherId, divisionId, academicPeriodId, "OF-2026-001", proposedDate, 10, 5, assignmentDate);

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
    }

    [Fact]
    public void Constructor_TrimOfficialLetterNumber()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "   OF-2026-001   ", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        Assert.Equal("OF-2026-001", academicLoad.OfficialLetterNumber);
    }
    [Fact]
    public void Constructor_EmptyTenantId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
    }
    [Fact]
    public void Constructor_EmptyTeacherId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
    }
    [Fact]
    public void Constructor_EmptyDivisionId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
    }
    [Fact]
    public void Constructor_EmptyAcademicPeriodId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
    }
    [Fact]
    public void Constructor_EmptyOfficialLetterNumber_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", DateTime.UtcNow, 10, 5, DateTime.UtcNow));
    }
    [Fact]
    public void Constructor_NegativeClassHours_ThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, -1, 5, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_NegativeSupportHours_ThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, -1, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_ZeroClassHoursAndSupportHours_DoesNotThrow()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 0, 0, DateTime.UtcNow);
        Assert.Equal(0, academicLoad.ClassHours);
        Assert.Equal(0, academicLoad.SupportHours);
    }

    [Fact]
    public void Update_ValidData_UpdatesPropertiesAndSetsUpdatedAtUtc()
    {
        var tenantId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var divisionId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var newProposedDate = new DateTime(2026, 2, 1);
        var newAssignmentDate = new DateTime(2026, 2, 5);

        var AcademicLoad = new AcademicLoad(tenantId, teacherId, divisionId, academicPeriodId, "OF-2026-001", newProposedDate, 10, 5, newAssignmentDate);
        AcademicLoad.Update("OF-2026-002", newProposedDate, 12, 6, newAssignmentDate);

        Assert.Equal("OF-2026-002", AcademicLoad.OfficialLetterNumber);
        Assert.Equal(newProposedDate, AcademicLoad.ProposedDate);
        Assert.Equal(12, AcademicLoad.ClassHours);
        Assert.Equal(6, AcademicLoad.SupportHours);
        Assert.Equal(newAssignmentDate, AcademicLoad.AssignmentDate);
        Assert.NotNull(AcademicLoad.UpdatedAtUtc);
    }

    [Fact]
    public void Update_EmptyOfficialLetterNumber_ThrowArgumentException()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => academicLoad.Update("", DateTime.UtcNow, 12, 6, DateTime.UtcNow));
    }

    [Fact]
    public void Update_NegativeClassHours_ThrowArgumentOutOfRangeException()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        Assert.Throws<ArgumentOutOfRangeException>(() => academicLoad.Update("OF-2026-002", DateTime.UtcNow, -1, 6, DateTime.UtcNow));
    }

    [Fact]
    public void Update_NegativeSupportHours_ThrowArgumentOutOfRangeException()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        Assert.Throws<ArgumentOutOfRangeException>(() => academicLoad.Update("OF-2026-002", DateTime.UtcNow, 12, -1, DateTime.UtcNow));
    }

    [Fact]
    public void Deactivate_StatusFalse()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        academicLoad.Deactivate();

        Assert.False(academicLoad.Status);
        Assert.NotNull(academicLoad.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_StatusTrue()
    {
        var academicLoad = new AcademicLoad(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "OF-2026-001", DateTime.UtcNow, 10, 5, DateTime.UtcNow);
        academicLoad.Activate();

        Assert.True(academicLoad.Status);
        Assert.NotNull(academicLoad.UpdatedAtUtc);
    }
}