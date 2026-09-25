using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Domain.Entities;

public class StudyPlanSubjectTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var prerequisiteId = Guid.NewGuid();

        // Act
        var studyPlanSubject = new StudyPlanSubject(
            tenantId,
            studyPlanId,
            subjectId,
            1,
            6,
            true,
            prerequisiteId);

        // Assert
        Assert.NotEqual(Guid.Empty, studyPlanSubject.Id);
        Assert.Equal(tenantId, studyPlanSubject.TenantId);
        Assert.Equal(studyPlanId, studyPlanSubject.StudyPlanId);
        Assert.Equal(subjectId, studyPlanSubject.SubjectId);
        Assert.Equal(prerequisiteId, studyPlanSubject.PrerequisiteSubjectId);
        Assert.Equal(1, studyPlanSubject.Semester);
        Assert.Equal(6, studyPlanSubject.Credits);
        Assert.True(studyPlanSubject.IsRequired);
        Assert.True(studyPlanSubject.Status);
        Assert.NotEqual(default, studyPlanSubject.CreatedAtUtc);
        Assert.Null(studyPlanSubject.UpdatedAtUtc);
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
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, false));
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
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, false));
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
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, false));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidSemester_ShouldThrowArgumentOutOfRangeException(int semester)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, semester, 6, false));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCredits_ShouldThrowArgumentOutOfRangeException(int credits)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, credits, false));
    }

    [Fact]
    public void Constructor_WithSelfReferencePrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, true, subjectId));
    }

    [Fact]
    public void Constructor_WithIsRequiredTrueAndNullPrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, true, null));
    }

    [Fact]
    public void Constructor_WithIsRequiredFalseAndNotNullPrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var prerequisiteId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new StudyPlanSubject(tenantId, studyPlanId, subjectId, 1, 6, false, prerequisiteId));
    }

    [Fact]
    public void SoftDelete_ShouldSetStatusToFalse()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act
        studyPlanSubject.SoftDelete();

        // Assert
        Assert.False(studyPlanSubject.Status);
    }

    [Fact]
    public void SoftDelete_ShouldSetUpdatedAtUtc()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act
        studyPlanSubject.SoftDelete();

        // Assert
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Fact]
    public void Restore_ShouldSetStatusToTrue()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();
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
        var studyPlanSubject = CreateValidEntity();
        studyPlanSubject.SoftDelete();

        // Act
        studyPlanSubject.Restore();

        // Assert
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdate()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();
        var newPrerequisiteId = Guid.NewGuid();

        // Act
        studyPlanSubject.Update(3, 8, true, newPrerequisiteId);

        // Assert
        Assert.Equal(3, studyPlanSubject.Semester);
        Assert.Equal(8, studyPlanSubject.Credits);
        Assert.True(studyPlanSubject.IsRequired);
        Assert.Equal(newPrerequisiteId, studyPlanSubject.PrerequisiteSubjectId);
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_WithInvalidSemester_ShouldThrowArgumentOutOfRangeException(int semester)
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            studyPlanSubject.Update(semester, 8, false, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_WithInvalidCredits_ShouldThrowArgumentOutOfRangeException(int credits)
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            studyPlanSubject.Update(3, credits, false, null));
    }

    [Fact]
    public void Update_WithSelfReferencePrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            studyPlanSubject.Update(3, 8, true, studyPlanSubject.SubjectId));
    }

    [Fact]
    public void Update_WithIsRequiredTrueAndNullPrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            studyPlanSubject.Update(3, 8, true, null));
    }

    [Fact]
    public void Update_WithIsRequiredFalseAndNotNullPrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var studyPlanSubject = CreateValidEntity();
        var prerequisiteId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            studyPlanSubject.Update(3, 8, false, prerequisiteId));
    }

    private static StudyPlanSubject CreateValidEntity()
    {
        return new StudyPlanSubject(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            6,
            false,
            null);
    }
}