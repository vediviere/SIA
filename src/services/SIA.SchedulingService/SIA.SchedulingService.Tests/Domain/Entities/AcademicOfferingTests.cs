using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class AcademicOfferingTests
{
    [Fact]
    public void Constructor_ValidData_CreateAcademicOffering()
    {
        var tenantId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var academicLoad = Guid.NewGuid();
        var academicOffering = new AcademicOffering(tenantId, groupId, subjectId, academicLoad, "Aceptado");

        Assert.NotEqual(Guid.Empty, academicOffering.Id);
        Assert.Equal(tenantId, academicOffering.TenantId);
        Assert.Equal(groupId, academicOffering.GroupId);
        Assert.Equal(subjectId, academicOffering.SubjectId);
        Assert.Equal(academicLoad, academicOffering.AcademicLoadId);
        Assert.Equal("Aceptado", academicOffering.OfferingStatus);
        Assert.True(academicOffering.Status);
        Assert.Null(academicOffering.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_EmptyTenantId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Aceptado"));
    }
    [Fact]
    public void Constructor_EmptygroupId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Aceptado"));
    }
    [Fact]
    public void Constructor_EmptysubjectId_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Aceptado"));
    }
    [Fact]
    public void Constructor_EmptyacademicLoad_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "Aceptado"));
    }
    [Fact]
    public void Constructor_EmptyOfferingStatus_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ""));
    }
    [Fact]
    public void Constructor_TrimOfferingStatus()
    {
        var academicOffering = new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "  Aceptado  ");
        Assert.Equal("Aceptado", academicOffering.OfferingStatus);
    }

    [Fact]
    public void Update_AcademicOffering()
    {
        var academicOffering = new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Aceptado");
        academicOffering.Update("No aceptado");

        Assert.Equal("No aceptado", academicOffering.OfferingStatus);
        Assert.NotNull(academicOffering.UpdatedAtUtc);
    }
    [Fact]
    public void Update_EmptyOfferingStatus_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ""));
    }

    [Fact]
    public void Deactivate_StatusFalse()
    {
        var academicOffering = new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Aceptado");
        academicOffering.Deactivate();

        Assert.False(academicOffering.Status);
        Assert.NotNull(academicOffering.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_StatusTrue()
    {
        var academicOffering = new AcademicOffering(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Aceptado");
        academicOffering.Activate();

        Assert.True(academicOffering.Status);
        Assert.NotNull(academicOffering.UpdatedAtUtc);
    }

}