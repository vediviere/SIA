using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Domain.Entities;

public class StudyPlanTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateStudyPlan()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act
        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "PLAN-2026",
            "Plan de Estudios Ingeniería en Sistemas",
            "1.0",
            effectiveFrom);

        // Assert
        Assert.NotEqual(Guid.Empty, studyPlan.Id);
        Assert.Equal(tenantId, studyPlan.TenantId);
        Assert.Equal(educationalProgramId, studyPlan.EducationalProgramId);
        Assert.Equal("PLAN-2026", studyPlan.Code);
        Assert.Equal("Plan de Estudios Ingeniería en Sistemas", studyPlan.Name);
        Assert.Equal("1.0", studyPlan.Version);
        Assert.Equal(effectiveFrom, studyPlan.EffectiveFrom);
        Assert.True(studyPlan.Status);
        Assert.NotEqual(default, studyPlan.CreatedAtUtc);
        Assert.Null(studyPlan.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.Empty;
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                "PLAN-2026",
                "Plan de Estudios Ingeniería en Sistemas",
                "1.0",
                effectiveFrom));
    }

    [Fact]
    public void Constructor_WithEmptyEducationalProgramId_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.Empty;
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                "PLAN-2026",
                "Plan de Estudios Ingeniería en Sistemas",
                "1.0",
                effectiveFrom));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCode_ShouldThrowArgumentException(string code)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                code,
                "Plan de Estudios Ingeniería en Sistemas",
                "1.0",
                effectiveFrom));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string name)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                "PLAN-2026",
                name,
                "1.0",
                effectiveFrom));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidVersion_ShouldThrowArgumentException(string version)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                "PLAN-2026",
                "Plan de Estudios Ingeniería en Sistemas",
                version,
                effectiveFrom));
    }

    [Fact]
    public void Constructor_WithDefaultEffectiveFrom_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = default(DateOnly);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlan(
                tenantId,
                educationalProgramId,
                "PLAN-2026",
                "Plan de Estudios Ingeniería en Sistemas",
                "1.0",
                effectiveFrom));
    }

    [Fact]
    public void Constructor_WithCodeContainingSpaces_ShouldNormalizeCode()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);
        var code = "  plan-2026  ";

        // Act
        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            code,
            "Plan de Estudios Ingeniería en Sistemas",
            "1.0",
            effectiveFrom);

        // Assert
        Assert.Equal("PLAN-2026", studyPlan.Code);
    }

    [Fact]
    public void Constructor_WithNameContainingSpaces_ShouldTrimName()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);
        var name = "  Plan de Estudios Ingeniería en Sistemas  ";

        // Act
        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "PLAN-2026",
            name,
            "1.0",
            effectiveFrom);

        // Assert
        Assert.Equal(
            "Plan de Estudios Ingeniería en Sistemas",
            studyPlan.Name);
    }

    [Fact]
    public void Constructor_WithVersionContainingSpaces_ShouldTrimVersion()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var effectiveFrom = new DateOnly(2026, 8, 1);
        var version = "  1.0  ";

        // Act
        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "PLAN-2026",
            "Plan de Estudios Ingeniería en Sistemas",
            version,
            effectiveFrom);

        // Assert
        Assert.Equal("1.0", studyPlan.Version);
    }

    [Fact]
    public void Desactivate_ShouldSetStatusToFalse()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act
        studyPlan.Desactivate();

        // Assert
        Assert.False(studyPlan.Status);
    }

    [Fact]
    public void Desactivate_ShouldSetUpdatedAtUtc()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act
        studyPlan.Desactivate();

        // Assert
        Assert.NotNull(studyPlan.UpdatedAtUtc);
    }

    [Fact]
    public void Activate_ShouldSetStatusToTrue()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();
        studyPlan.Desactivate();

        // Act
        studyPlan.Activate();

        // Assert
        Assert.True(studyPlan.Status);
    }

    [Fact]
    public void Activate_ShouldSetUpdatedAtUtc()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();
        studyPlan.Desactivate();

        // Act
        studyPlan.Activate();

        // Assert
        Assert.NotNull(studyPlan.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateStudyPlan()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        var newEffectiveFrom = new DateOnly(2027, 1, 15);

        // Act
        studyPlan.UpdateDetails(
            "PLAN-2027",
            "Plan de Estudios Ingeniería en Software",
            "2.0",
            newEffectiveFrom);

        // Assert
        Assert.Equal("PLAN-2027", studyPlan.Code);
        Assert.Equal(
            "Plan de Estudios Ingeniería en Software",
            studyPlan.Name);
        Assert.Equal("2.0", studyPlan.Version);
        Assert.Equal(newEffectiveFrom, studyPlan.EffectiveFrom);
        Assert.NotNull(studyPlan.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidCode_ShouldThrowArgumentException(string code)
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            studyPlan.UpdateDetails(
                code,
                "Nuevo Plan de Estudios",
                "2.0",
                new DateOnly(2027, 1, 15)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidName_ShouldThrowArgumentException(string name)
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            studyPlan.UpdateDetails(
                "PLAN-2027",
                name,
                "2.0",
                new DateOnly(2027, 1, 15)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidVersion_ShouldThrowArgumentException(string version)
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            studyPlan.UpdateDetails(
                "PLAN-2027",
                "Nuevo Plan de Estudios",
                version,
                new DateOnly(2027, 1, 15)));
    }

    [Fact]
    public void UpdateDetails_WithDefaultEffectiveFrom_ShouldThrowArgumentException()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            studyPlan.UpdateDetails(
                "PLAN-2027",
                "Nuevo Plan de Estudios",
                "2.0",
                default));
    }

    [Fact]
    public void UpdateDetails_WithCodeContainingSpaces_ShouldNormalizeCode()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act
        studyPlan.UpdateDetails(
            "  plan-2027  ",
            "Nuevo Plan de Estudios",
            "2.0",
            new DateOnly(2027, 1, 15));

        // Assert
        Assert.Equal("PLAN-2027", studyPlan.Code);
    }

    [Fact]
    public void UpdateDetails_WithNameContainingSpaces_ShouldTrimName()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act
        studyPlan.UpdateDetails(
            "PLAN-2027",
            "  Nuevo Plan de Estudios  ",
            "2.0",
            new DateOnly(2027, 1, 15));

        // Assert
        Assert.Equal("Nuevo Plan de Estudios", studyPlan.Name);
    }

    [Fact]
    public void UpdateDetails_WithVersionContainingSpaces_ShouldTrimVersion()
    {
        // Arrange
        var studyPlan = CreateValidStudyPlan();

        // Act
        studyPlan.UpdateDetails(
            "PLAN-2027",
            "Nuevo Plan de Estudios",
            "  2.0  ",
            new DateOnly(2027, 1, 15));

        // Assert
        Assert.Equal("2.0", studyPlan.Version);
    }

    private static StudyPlan CreateValidStudyPlan()
    {
        return new StudyPlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PLAN-2026",
            "Plan de Estudios Ingeniería en Sistemas",
            "1.0",
            new DateOnly(2026, 8, 1));
    }
}