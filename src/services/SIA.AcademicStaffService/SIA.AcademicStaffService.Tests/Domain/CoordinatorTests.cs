using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Domain;

public sealed class CoordinatorTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveCoordinator()
    {
        var tenantId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var coordinator = new Coordinator(tenantId, personId);

        Assert.NotEqual(Guid.Empty, coordinator.Id);
        Assert.Equal(tenantId, coordinator.TenantId);
        Assert.Equal(personId, coordinator.PersonId);
        Assert.True(coordinator.Status);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Coordinator(Guid.Empty, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptyPersonId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Coordinator(Guid.NewGuid(), Guid.Empty));
    }

    [Fact]
    public void Deactivate_ShouldSetStatusFalse()
    {
        var coordinator = new Coordinator(Guid.NewGuid(), Guid.NewGuid());

        coordinator.Deactivate();

        Assert.False(coordinator.Status);
        Assert.NotNull(coordinator.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusTrue()
    {
        var coordinator = new Coordinator(Guid.NewGuid(), Guid.NewGuid());
        coordinator.Deactivate();

        coordinator.Activate();

        Assert.True(coordinator.Status);
    }
}