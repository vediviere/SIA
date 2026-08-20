using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Tests.Domain;

public sealed class ClassroomTypeTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveClassroomType()
    {
        var type = new ClassroomType(_validTenantId, " lab-comp ", " Laboratorio de Cómputo ", " Observación ");

        Assert.NotEqual(Guid.Empty, type.Id);
        Assert.Equal(_validTenantId, type.TenantId);
        Assert.Equal("LAB-COMP", type.Code); 
        Assert.Equal("Laboratorio de Cómputo", type.Name);
        Assert.Equal("Observación", type.Description);
        Assert.True(type.Status);
        Assert.NotEqual(default, type.CreatedAtUtc);
        Assert.Null(type.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new ClassroomType(Guid.Empty, "LAB", "Name", "Desc"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyCode_ShouldThrowArgumentException(string? invalidCode)
    {
        Assert.Throws<ArgumentException>(() =>
            new ClassroomType(_validTenantId, invalidCode!, "Name", "Desc"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException(string? invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
            new ClassroomType(_validTenantId, "LAB", invalidName!, "Desc"));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdatePropertiesAndTimestamp()
    {
        var type = new ClassroomType(_validTenantId, "LAB-01", "Lab", "Desc");
        type.Update("LAB-02", "Lab 2", "Desc 2");
        Assert.Equal("LAB-02", type.Code);
        Assert.Equal("Lab 2", type.Name);
        Assert.Equal("Desc 2", type.Description);
        Assert.NotNull(type.UpdatedAtUtc);
    }

    [Fact]
    public void SoftDelete_ShouldChangeStatusToFalse()
    {
        var type = new ClassroomType(_validTenantId, "LAB", "Name", "Desc");
        type.SoftDelete();
        Assert.False(type.Status);
        Assert.NotNull(type.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldChangeStatusToTrue()
    {
        var type = new ClassroomType(_validTenantId, "LAB", "Name", "Desc");
        type.SoftDelete();
        type.Restore();
        Assert.True(type.Status);
        Assert.NotNull(type.UpdatedAtUtc);
    }
}