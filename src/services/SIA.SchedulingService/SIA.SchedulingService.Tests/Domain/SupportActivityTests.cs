using SIA.SchedulingService.Domain.Entities;
using System;
using Xunit;

namespace SIA.SchedulingService.Tests.Domain;

public sealed class SupportActivityTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveSupportActivity()
    {
        var tenantId = Guid.NewGuid();
        var activityName = "Tutoría de Programación en C#";
        var observation = "Apoyo de regularización";

        var supportActivity = new SupportActivity(tenantId, activityName, observation);

        Assert.NotEqual(Guid.Empty, supportActivity.Id);
        Assert.Equal(tenantId, supportActivity.TenantId);
        Assert.Equal(activityName, supportActivity.Activity);
        Assert.Equal(observation, supportActivity.Observation);
        Assert.True(supportActivity.Status);
        Assert.NotEqual(default, supportActivity.CreatedAtUtc);
        Assert.Null(supportActivity.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_ShouldTrimActivityAndObservation()
    {
        var tenantId = Guid.NewGuid();
        var supportActivity = new SupportActivity(tenantId, "   Tutoría de Programación   ", "   Observaciones   ");
        Assert.Equal("Tutoría de Programación", supportActivity.Activity);
        Assert.Equal("Observaciones", supportActivity.Observation);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new SupportActivity(Guid.Empty, "Tutoría", "Observación"));
    }
    [Fact]
    public void Constructor_WithEmptyActivity_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new SupportActivity(Guid.NewGuid(), "", "Observación"));
    }

    [Fact]
    public void SoftDelete_ShouldChangeStatusToFalse()
    {
        var supportActivity = new SupportActivity(Guid.NewGuid(), "Tutoría", "Observación");
        supportActivity.SoftDelete();
        Assert.False(supportActivity.Status);
        Assert.NotNull(supportActivity.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldChangeStatusToTrue()
    {
        var supportActivity = new SupportActivity(Guid.NewGuid(), "Tutoría", "Observación");
        supportActivity.SoftDelete();
        supportActivity.Restore();
        Assert.True(supportActivity.Status);
        Assert.NotNull(supportActivity.UpdatedAtUtc);
    }
}