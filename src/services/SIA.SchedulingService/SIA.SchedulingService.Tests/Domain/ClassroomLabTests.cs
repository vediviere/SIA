using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Domain;

public sealed class ClassroomLabTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validBuildingId = Guid.NewGuid();
    private readonly Guid _validClassroomTypeId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveClassroomLab()
    {
        var lab = new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, " lab-01 ", " Laboratorio de Redes ", 30, " Observación ");

        Assert.NotEqual(Guid.Empty, lab.Id);
        Assert.Equal(_validTenantId, lab.TenantId);
        Assert.Equal(_validBuildingId, lab.BuildingId);
        Assert.Equal(_validClassroomTypeId, lab.ClassroomTypeId);
        Assert.Equal("LAB-01", lab.Code);
        Assert.Equal("Laboratorio de Redes", lab.Name);
        Assert.Equal(30, lab.Capacity);
        Assert.Equal("Observación", lab.Description);
        Assert.True(lab.Status);
        Assert.NotEqual(default, lab.CreatedAtUtc);
        Assert.Null(lab.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new ClassroomLab(Guid.Empty, _validBuildingId, _validClassroomTypeId, "LAB", "Name", 30, "Desc"));
    }

    [Fact]
    public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, "", "Name", 30, "Desc"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCapacity_ShouldThrowArgumentOutOfRangeException(int invalidCapacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, "LAB", "Name", invalidCapacity, "Desc"));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdatePropertiesAndTimestamp()
    {
        var lab = new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, "LAB-01", "Lab", 30, "Desc");
        lab.Update("LAB-02", "Lab 2", 40, "Desc 2");
        Assert.Equal("LAB-02", lab.Code);
        Assert.Equal("Lab 2", lab.Name);
        Assert.Equal(40, lab.Capacity);
        Assert.Equal("Desc 2", lab.Description);
        Assert.NotNull(lab.UpdatedAtUtc);
    }

    [Fact]
    public void SoftDelete_ShouldChangeStatusToFalse()
    {
        var lab = new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, "LAB", "Name", 30, "Desc");
        lab.SoftDelete();
        Assert.False(lab.Status);
        Assert.NotNull(lab.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldChangeStatusToTrue()
    {
        var lab = new ClassroomLab(_validTenantId, _validBuildingId, _validClassroomTypeId, "LAB", "Name", 30, "Desc");
        lab.SoftDelete();
        lab.Restore();
        Assert.True(lab.Status);
        Assert.NotNull(lab.UpdatedAtUtc);
    }
}