using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Domain;

public sealed class TeacherTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveTeacher()
    {
        var tenantId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var teacher = new Teacher(tenantId, personId, " Ingeniero de Software ", " Tiempo completo ", 40);

        Assert.NotEqual(Guid.Empty, teacher.Id);
        Assert.Equal(tenantId, teacher.TenantId);
        Assert.Equal(personId, teacher.PersonId);
        Assert.Equal("Ingeniero de Software", teacher.ProfessionalProfile);
        Assert.Equal("Tiempo completo", teacher.ContractType);
        Assert.Equal(40, teacher.ContractHours);
        Assert.True(teacher.Status);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Teacher(Guid.Empty, Guid.NewGuid(), "Perfil", "Tipo", 40));
    }

    [Fact]
    public void Constructor_WithEmptyPersonId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Teacher(Guid.NewGuid(), Guid.Empty, "Perfil", "Tipo", 40));
    }

    [Fact]
    public void Constructor_WithEmptyProfessionalProfile_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Teacher(Guid.NewGuid(), Guid.NewGuid(), "", "Tipo", 40));
    }

    [Fact]
    public void Constructor_WithEmptyContractType_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "", 40));
    }

    [Fact]
    public void Constructor_WithZeroContractHours_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "Tipo", 0));
    }

    [Fact]
    public void Constructor_WithNegativeContractHours_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "Tipo", -5));
    }

    [Fact]
    public void Constructor_WithoutProgramId_ShouldAllowNull()
    {
        var teacher = CreateValidTeacher();

        Assert.Null(teacher.ProgramId);
    }

    [Fact]
    public void Constructor_WithProgramId_ShouldSetProgramId()
    {
        var programId = Guid.NewGuid();

        var teacher = new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Perfil", "Tipo", 40, programId);

        Assert.Equal(programId, teacher.ProgramId);
    }

    [Fact]
    public void AssignProgram_ShouldUpdateProgramId()
    {
        var teacher = CreateValidTeacher();
        var programId = Guid.NewGuid();

        teacher.AssignProgram(programId);

        Assert.Equal(programId, teacher.ProgramId);
        Assert.NotNull(teacher.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        var teacher = CreateValidTeacher();

        teacher.Update("Nuevo perfil", "Medio tiempo", 20);

        Assert.Equal("Nuevo perfil", teacher.ProfessionalProfile);
        Assert.Equal("Medio tiempo", teacher.ContractType);
        Assert.Equal(20, teacher.ContractHours);
        Assert.NotNull(teacher.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithZeroContractHours_ShouldThrowArgumentOutOfRangeException()
    {
        var teacher = CreateValidTeacher();

        Assert.Throws<ArgumentOutOfRangeException>(() => teacher.Update("Perfil", "Tipo", 0));
    }

    [Fact]
    public void Deactivate_ShouldSetStatusFalse()
    {
        var teacher = CreateValidTeacher();

        teacher.Deactivate();

        Assert.False(teacher.Status);
        Assert.NotNull(teacher.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusTrue()
    {
        var teacher = CreateValidTeacher();
        teacher.Deactivate();

        teacher.Activate();

        Assert.True(teacher.Status);
    }

    private static Teacher CreateValidTeacher()
    {
        return new Teacher(Guid.NewGuid(), Guid.NewGuid(), "Ingeniero de Software", "Tiempo completo", 40);
    }
}