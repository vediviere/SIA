using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Domain;

public sealed class TeacherCompetencyTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateActiveTeacherCompetency()
    {
        var tenantId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var competencyId = Guid.NewGuid();

        var competency = new TeacherCompetency(tenantId, teacherId, competencyId, "Avanzado", "Validado");

        Assert.NotEqual(Guid.Empty, competency.TeacherCompetencyId);
        Assert.Equal(tenantId, competency.TenantId);
        Assert.Equal(teacherId, competency.TeacherId);
        Assert.Equal(competencyId, competency.CompetencyId);
        Assert.Equal("Avanzado", competency.Level);
        Assert.Equal("Validado", competency.Stage);
        Assert.True(competency.Status);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeacherCompetency(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Avanzado", "Validado"));
    }

    [Fact]
    public void Constructor_WithEmptyTeacherId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeacherCompetency(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Avanzado", "Validado"));
    }

    [Fact]
    public void Constructor_WithEmptyCompetencyId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TeacherCompetency(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "Avanzado", "Validado"));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        var competency = CreateValidTeacherCompetency();

        competency.Update("Intermedio", "Pendiente");

        Assert.Equal("Intermedio", competency.Level);
        Assert.Equal("Pendiente", competency.Stage);
        Assert.NotNull(competency.UpdatedAtUtc);
    }

    [Fact]
    public void Deactivate_ShouldSetStatusFalse()
    {
        var competency = CreateValidTeacherCompetency();

        competency.Deactivate();

        Assert.False(competency.Status);
        Assert.NotNull(competency.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusTrue()
    {
        var competency = CreateValidTeacherCompetency();
        competency.Deactivate();

        competency.Activate();

        Assert.True(competency.Status);
    }

    private static TeacherCompetency CreateValidTeacherCompetency()
    {
        return new TeacherCompetency(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Avanzado", "Validado");
    }
}