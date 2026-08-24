using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Domain.Entities;

public class StudyPlanSubjectTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateStudyPlanSubject()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act
        var studyPlanSubject = new StudyPlanSubject(
            tenantId,
            studyPlanId,
            subjectId,
            1,
            6,
            true);

        // Assert
        Assert.NotEqual(Guid.Empty, studyPlanSubject.Id);
        Assert.Equal(tenantId, studyPlanSubject.TenantId);
        Assert.Equal(studyPlanId, studyPlanSubject.StudyPlanId);
        Assert.Equal(subjectId, studyPlanSubject.SubjectId);
        Assert.Equal(1, studyPlanSubject.Semester);
        Assert.Equal(6, studyPlanSubject.Credits);
        Assert.True(studyPlanSubject.IsRequired);
        Assert.True(studyPlanSubject.Status);
        Assert.NotEqual(default, studyPlanSubject.CreatedAtUtc);
        Assert.Null(studyPlanSubject.UpdatedAtUtc);

        Assert.Null(studyPlanSubject.Subject);
        Assert.Null(studyPlanSubject.StudyPlan);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.Empty;
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlanSubject(
                tenantId,
                studyPlanId,
                subjectId,
                1,
                6,
                true));
    }

    [Fact]
    public void Constructor_WithEmptyStudyPlanId_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.Empty;
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlanSubject(
                tenantId,
                studyPlanId,
                subjectId,
                1,
                6,
                true));
    }

    [Fact]
    public void Constructor_WithEmptySubjectId_ShouldThrowArgumentException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new StudyPlanSubject(
                tenantId,
                studyPlanId,
                subjectId,
                1,
                6,
                true));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidSemester_ShouldThrowArgumentOutOfRangeException(
        int semester)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudyPlanSubject(
                tenantId,
                studyPlanId,
                subjectId,
                semester,
                6,
                true));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCredits_ShouldThrowArgumentOutOfRangeException(
        int credits)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudyPlanSubject(
                tenantId,
                studyPlanId,
                subjectId,
                1,
                credits,
                true));
    }

    [Fact]
    public void Constructor_WithIsRequiredFalse_ShouldPreserveValue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act
        var studyPlanSubject = new StudyPlanSubject(
            tenantId,
            studyPlanId,
            subjectId,
            1,
            6,
            false);

        // Assert
        Assert.False(studyPlanSubject.IsRequired);
    }

    [Fact]
    public void SoftDelete_ShouldSetStatusToFalse()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act
        studyPlanSubject.SoftDelete();

        // Assert
        Assert.False(studyPlanSubject.Status);
    }

    [Fact]
    public void SoftDelete_ShouldSetUpdatedAtUtc()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act
        studyPlanSubject.SoftDelete();

        // Assert
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldSetStatusToTrue()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();
        studyPlanSubject.SoftDelete();

        // Act
        studyPlanSubject.Restore();

        // Assert
        Assert.True(studyPlanSubject.Status);
    }

    [Fact]
    public void Restore_ShouldSetUpdatedAtUtc()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();
        studyPlanSubject.SoftDelete();

        // Act
        studyPlanSubject.Restore();

        // Assert
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateStudyPlanSubject()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act
        studyPlanSubject.Update(
            3,
            8,
            false);

        // Assert
        Assert.Equal(3, studyPlanSubject.Semester);
        Assert.Equal(8, studyPlanSubject.Credits);
        Assert.False(studyPlanSubject.IsRequired);
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_WithInvalidSemester_ShouldThrowArgumentOutOfRangeException(
        int semester)
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            studyPlanSubject.Update(
                semester,
                8,
                false));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_WithInvalidCredits_ShouldThrowArgumentOutOfRangeException(
        int credits)
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            studyPlanSubject.Update(
                3,
                credits,
                false));
    }

    [Fact]
    public void Update_WithIsRequiredTrue_ShouldPreserveValue()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act
        studyPlanSubject.Update(
            3,
            8,
            true);

        // Assert
        Assert.True(studyPlanSubject.IsRequired);
    }

    [Fact]
    public void Update_WithIsRequiredFalse_ShouldPreserveValue()
    {
        // Arrange
        var studyPlanSubject = CreateValidStudyPlanSubject();

        // Act
        studyPlanSubject.Update(
            3,
            8,
            false);

        // Assert
        Assert.False(studyPlanSubject.IsRequired);
    }

    private static StudyPlanSubject CreateValidStudyPlanSubject()
    {
        return new StudyPlanSubject(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            6,
            true);
    }
}