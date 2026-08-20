using SIA.SchedulingService.Domain.Entities;
namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class TeachingSupportHourTest
{
    [Fact]
    public void Constructor_ValidData_Create_TeachingSupportHour()
    {
        var tenantId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var academicLoadId = Guid.NewGuid();

        var teachingSupportHour = new TeachingSupportHour(tenantId, activityId, academicLoadId, 10);
        Assert.NotEqual(Guid.Empty, teachingSupportHour.Id);
        Assert.Equal(tenantId, teachingSupportHour.TenantId);
        Assert.Equal(activityId, teachingSupportHour.ActivityId);
        Assert.Equal(academicLoadId, teachingSupportHour.AcademicLoadId);
        Assert.Equal(10, teachingSupportHour.Hours);
        Assert.True(teachingSupportHour.Status);
        Assert.NotNull(teachingSupportHour.CreatedAtUtc);
        Assert.Null(teachingSupportHour.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_EmptyTenantId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeachingSupportHour(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 10));
    }
    [Fact]
    public void Constructor_EmptyactivityId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeachingSupportHour(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), 10));
    }
    [Fact]
    public void Constructor_EmptyacademicLoadId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 10));
    }
    [Fact]
    public void Constructor_CeroHours_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0));
    }
    [Fact]
    public void Constructor_NegativeHours_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -1));
    }

    [Fact]
    public void Update_teachingSupportHour()
    {
        var teachingSupportHour = new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10);
        teachingSupportHour.Update(20);

        Assert.Equal(20, teachingSupportHour.Hours);
        Assert.NotNull(teachingSupportHour.UpdatedAtUtc);
    }
    [Fact]
    public void Update_CeroHours_ArgumentOutOfRangeException()
    {
        var teachingSupportHour = new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => teachingSupportHour.Update(0));
    }
    [Fact]
    public void Update_NegativeHours_ArgumentOutOfRangeException()
    {
        var teachingSupportHour = new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => teachingSupportHour.Update(-1));
    }

    [Fact]
    public void Deactivate_StatusFalse()
    {
        var teachingSupportHour = new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10);
        teachingSupportHour.Deactivate();
        Assert.False(teachingSupportHour.Status);
        Assert.NotNull(teachingSupportHour.UpdatedAtUtc);
    }
    [Fact]
    public void Activate_StatusTrue()
    {
        var teachingSupportHour = new TeachingSupportHour(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10);
        teachingSupportHour.Deactivate();

        teachingSupportHour.Activate();
        Assert.True(teachingSupportHour.Status);
        Assert.NotNull(teachingSupportHour.UpdatedAtUtc);
    }
}