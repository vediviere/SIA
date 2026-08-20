using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class BuildingTests
{
    [Fact]
    public void Constructor_ValidData_CreateBuilding()
    {
        var tenantId = Guid.NewGuid();
        var building = new Building(tenantId, "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");

        Assert.NotEqual(Guid.Empty, building.Id);
        Assert.Equal(tenantId, building.TenantId);
        Assert.Equal("EDIFICIO ISIC-IFOR 2345", building.Code);
        Assert.Equal("Edificio 1", building.Name);
        Assert.True(building.Status);
        Assert.Null(building.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_EmptyTenantId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building(Guid.Empty, "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR"));
    }

    [Fact]
    public void Constructor_EmptyCode_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building(Guid.NewGuid(), "", "EDIFICIO ISIC - A", "Edificio para carerras ISIC y IFOR"));
    } 
    
    [Fact]
    public void Constructor_EmptyName_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "", "Edificio para carerras ISIC y IFOR"));
    }

    [Fact]
    public void Constructor_TrimAndToUpperCodeAndTrimName()
    {
        var building = new Building(Guid.NewGuid(), "   Edificio isic-ifor 2345   ", "  Edificio 1  ", "Edificio para carerras ISIC y IFOR");

        Assert.Equal("EDIFICIO ISIC-IFOR 2345", building.Code);
        Assert.Equal("Edificio 1", building.Name);
    }

    [Fact]
    public void Constructor_WhitespaceCode_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building(Guid.NewGuid(), "   ", "EDIFICIO ISIC - A", "Edificio para carerras ISIC y IFOR"));
    }

    [Fact]
    public void Constructor_WhitespaceName_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "    ", "Edificio para carerras ISIC y IFOR"));
    }

    [Fact]
    public void Constructor_TrimDescription()
    {
        var building = new Building(Guid.NewGuid(), "   Edificio isic-ifor 2345   ", "  Edificio 1  ", "   Edificio para carerras ISIC y IFOR   ");
        Assert.Equal("Edificio para carerras ISIC y IFOR", building.Description);
    }

    [Fact]
    public void Constructor_NullDescription_DescriptionIsEmpty()
    {
        var building = new Building(Guid.NewGuid(), "   Edificio isic-ifor 2345   ", "  Edificio 1  ", null);
        Assert.Equal(String.Empty, building.Description);
    }

    [Fact]
    public void Constructor_Update()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");

        building.Update("EDIFICIO ISIC-IFOR 234567", "Edificio 2", "Cambio de descripcion");

        Assert.Equal("EDIFICIO ISIC-IFOR 234567", building.Code);
        Assert.Equal("Edificio 2", building.Name);
        Assert.Equal("Cambio de descripcion", building.Description);
        Assert.NotNull(building.UpdatedAtUtc);
    }

    [Fact]
    public void Update_TrimAndToUpperCodeAndTrimName()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");

        building.Update("   edificio isic-ifor 234567   ", "   Edificio 2   ", "Cambio de descripcion");
        Assert.Equal("EDIFICIO ISIC-IFOR 234567", building.Code);
        Assert.Equal("Edificio 2", building.Name);
    }

    [Fact]
    public void Update_EmptyCode_ThrowArgumentException()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");
        Assert.Throws<ArgumentException>(() => building.Update("", "Edificio 2", "Cambio de descripcion"));
    }

    [Fact]
    public void Update_EmptyName_ThrowArgumentException()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");
        Assert.Throws<ArgumentException>(() => building.Update("EDIFICIO ISIC-IFOR 234567", "", "Cambio de descripcion"));
    }

    [Fact]
    public void Deactivate_StatusFalse()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");

        building.Deactivate();
        Assert.False(building.Status);
    }

    [Fact]
    public void Activate_StatusTrue()
    {
        var building = new Building(Guid.NewGuid(), "Edificio ISIC-IFOR 2345", "Edificio 1", "Edificio para carerras ISIC y IFOR");

        building.Activate();
        Assert.True(building.Status);
    }
}