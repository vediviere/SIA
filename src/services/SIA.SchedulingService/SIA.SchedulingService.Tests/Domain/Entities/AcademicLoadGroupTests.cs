using SIA.SchedulingService.Domain.Entities.Academics;

namespace SIA.SchedulingService.Tests.Domain.Entities;

public sealed class AcademicLoadGroupTests
{
    private readonly Guid _validTenantId = Guid.NewGuid();
    private readonly Guid _validAcademicLoadId = Guid.NewGuid();
    private readonly Guid _validGroupId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_ShouldCreateAcademicLoadGroup()
    {
        var assignment = new AcademicLoadGroup(_validTenantId, _validAcademicLoadId, _validGroupId);

        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.Equal(_validTenantId, assignment.TenantId);
        Assert.Equal(_validAcademicLoadId, assignment.AcademicLoadId);
        Assert.Equal(_validGroupId, assignment.GroupId);
        Assert.Equal(2, assignment.Hours);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new AcademicLoadGroup(Guid.Empty, _validAcademicLoadId, _validGroupId));
    }

    [Fact]
    public void Constructor_WithEmptyGroupId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new AcademicLoadGroup(_validTenantId, _validAcademicLoadId, Guid.Empty));
    }
}