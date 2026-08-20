using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Domain;

public sealed class SupportScheduleTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validSupportHourId = Guid.NewGuid();
    private readonly Guid _validClassroomLabId = Guid.NewGuid();
    private readonly Guid _validAcademicPeriodId = Guid.NewGuid();
    private readonly DateTime _validStartTime = DateTime.UtcNow;
    private readonly DateTime _validEndTime = DateTime.UtcNow.AddHours(2);

    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveSupportSchedule()
    {
        var schedule = new SupportSchedule(
            _validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, " lunes ", _validStartTime, _validEndTime);

        Assert.NotEqual(Guid.Empty, schedule.Id);
        Assert.Equal(_validTenantId, schedule.TenantId);
        Assert.Equal(_validSupportHourId, schedule.SupportHourId);
        Assert.Equal(_validClassroomLabId, schedule.ClassroomLabId);
        Assert.Equal(_validAcademicPeriodId, schedule.AcademicPeriodId);
        Assert.Equal("LUNES", schedule.Day); 
        Assert.Equal(_validStartTime, schedule.StartTime);
        Assert.Equal(_validEndTime, schedule.EndTime);
        Assert.True(schedule.Status);
        Assert.NotEqual(default, schedule.CreatedAtUtc);
        Assert.Null(schedule.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyIds_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SupportSchedule(Guid.Empty, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime));
        Assert.Throws<ArgumentException>(() => new SupportSchedule(_validTenantId, Guid.Empty, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime));
        Assert.Throws<ArgumentException>(() => new SupportSchedule(_validTenantId, _validSupportHourId, Guid.Empty, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime));
        Assert.Throws<ArgumentException>(() => new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, Guid.Empty, "LUNES", _validStartTime, _validEndTime));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyDay_ShouldThrowArgumentException(string? invalidDay)
    {
        Assert.Throws<ArgumentException>(() =>
            new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, invalidDay!, _validStartTime, _validEndTime));
    }

    [Fact]
    public void Constructor_WhenStartTimeIsAfterOrEqualEndTime_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validStartTime));

        Assert.Throws<ArgumentException>(() =>
            new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validEndTime, _validStartTime));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdatePropertiesAndTimestamp()
    {
        var schedule = new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime);
        var newStartTime = DateTime.UtcNow.AddDays(1);
        var newEndTime = newStartTime.AddHours(1);

        schedule.Update("MARTES", newStartTime, newEndTime);

        Assert.Equal("MARTES", schedule.Day);
        Assert.Equal(newStartTime, schedule.StartTime);
        Assert.Equal(newEndTime, schedule.EndTime);
        Assert.NotNull(schedule.UpdatedAtUtc);
    }

    [Fact]
    public void SoftDelete_ShouldChangeStatusToFalse()
    {
        var schedule = new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime);

        schedule.SoftDelete();

        Assert.False(schedule.Status);
        Assert.NotNull(schedule.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldChangeStatusToTrue()
    {
        var schedule = new SupportSchedule(_validTenantId, _validSupportHourId, _validClassroomLabId, _validAcademicPeriodId, "LUNES", _validStartTime, _validEndTime);
        schedule.SoftDelete();

        schedule.Restore();

        Assert.True(schedule.Status);
        Assert.NotNull(schedule.UpdatedAtUtc);
    }
}