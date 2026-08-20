using SIA.SchedulingService.Domain.Entities;
namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class GroupTests
{
    [Fact]
    public void Contructor_ValidData_CreateGroup()
    {
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var group = new Group(tenantId, educationalProgramId, "Grupo A-ISIC", "Vespertino", 9);

        Assert.NotEqual(Guid.Empty, group.Id);
        Assert.Equal(tenantId, group.TenantId);
        Assert.Equal(educationalProgramId, group.EducationalProgramId);
        Assert.Equal("GRUPO A-ISIC", group.GroupName);
        Assert.Equal("VESPERTINO", group.Shift);
        Assert.Equal(9, group.Capacity);
        Assert.True(group.Status);
        Assert.Null(group.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_EmptyTenantId_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Group(Guid.Empty, Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 9));
    }
    [Fact]
    public void Constructor_EmptyeducationalProgramId_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Group(Guid.NewGuid(), Guid.Empty, "Grupo A-ISIC", "Vespertino", 9));
    }
    [Fact]
    public void Constructor_EmptyGroupName_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Group(Guid.NewGuid(), Guid.NewGuid(), "", "Vespertino", 9));
    }
    [Fact]
    public void Conatructor_EmptyShift_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "", 9));
    }
    [Fact]
    public void Constructor_TrimAndUpperGroupNameAndShift()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "  grupo A-ISIC  ", "  vespertino  ", 9);
        Assert.Equal("GRUPO A-ISIC", group.GroupName);
        Assert.Equal("VESPERTINO", group.Shift);
    }
    [Fact]
    public void Constructor_CeroCapacity_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 0));
    }
    [Fact]
    public void Constructor_NegativeCapacity_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", -1));
    }

    [Fact]
    public void Update_Group()
    {
        var group = new Group(Guid.NewGuid(),Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 9);

        group.Update("Grupo B-ISIC", "Matutino", 10);

        Assert.Equal("GRUPO B-ISIC", group.GroupName);
        Assert.Equal("MATUTINO", group.Shift);
        Assert.Equal(10, group.Capacity);
        Assert.NotNull(group.UpdatedAtUtc);
    }
    [Fact]
    public void Update_EmptyName_ThrowArgumentException()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 30);
        Assert.Throws<ArgumentException>(() => group.Update("", "Vespertino", 25));
    }
    [Fact]
    public void Update_EmptyShift_ThrowArgumentException()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Matutino", 30);
        Assert.Throws<ArgumentException>(() => group.Update("Grupo A-ISIC", "", 25));
    }
    [Fact]
    public void Update_CeroCapacity_ThrowArgumentOutOfRangeException()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A", "Matutino", 30);
        Assert.Throws<ArgumentOutOfRangeException>(() => group.Update("Grupo B", "Vespertino", 0));
    }
    [Fact]
    public void Update_NegativeCapacity_ThrowArgumentOutOfRangeException()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 9);
        Assert.Throws<ArgumentOutOfRangeException>(() => group.Update("Grupo A-ISIC", "Vespertino", -1));
    }

    [Fact]
    public void Deactivate_StatusFalse()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 9);
        group.Deactivate();
        Assert.False(group.Status);
        Assert.NotNull(group.UpdatedAtUtc);
    }
    [Fact]
    public void Activate_StatusTrue()
    {
        var group = new Group(Guid.NewGuid(), Guid.NewGuid(), "Grupo A-ISIC", "Vespertino", 9);
        group.Deactivate();

        group.Activate();   
        Assert.True(group.Status);
        Assert.NotNull(group.UpdatedAtUtc);
    }
}