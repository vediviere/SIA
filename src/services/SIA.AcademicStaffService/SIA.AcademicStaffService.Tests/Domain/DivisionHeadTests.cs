using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Domain;

public sealed class DivisionHeadTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveDivisionHead()
    {
        var tenantId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var divisionHead = new DivisionHead(tenantId, programId, personId);

        Assert.NotEqual(Guid.Empty, divisionHead.Id);
        Assert.Equal(tenantId, divisionHead.TenantId);
        Assert.Equal(programId, divisionHead.ProgramId);
        Assert.Equal(personId, divisionHead.PersonId);
        Assert.True(divisionHead.Status);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DivisionHead(Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptyProgramId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DivisionHead(Guid.NewGuid(), Guid.Empty, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptyPersonId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DivisionHead(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));
    }

    [Fact]
    public void Deactivate_ShouldSetStatusFalse()
    {
        var divisionHead = new DivisionHead(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        divisionHead.Deactivate();

        Assert.False(divisionHead.Status);
        Assert.NotNull(divisionHead.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusTrue()
    {
        var divisionHead = new DivisionHead(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        divisionHead.Deactivate();

        divisionHead.Activate();

        Assert.True(divisionHead.Status);
    }
}