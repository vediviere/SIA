using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlanSubjects;

public class SoftDeleteStudyPlanSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSoftDeleteStudyPlanSubject()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlanSubject = new StudyPlanSubject(tenantId, studyPlanId, subjectId, 3, 6, true);

        var studyPlanSubjectId = studyPlanSubject.Id;

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId,
                studyPlanSubjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlanSubject);

        dataStore.Setup(x => x.SoftDeleteStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new SoftDeleteStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,studyPlanSubjectId,correlationId,CancellationToken.None);

        // Assert
        Assert.False(studyPlanSubject.Status);
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);

        dataStore.Verify(
            x => x.SoftDeleteStudyPlanSubjectWithOutboxAsync(
                studyPlanSubject,
                It.IsAny<StudyPlanSubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudyPlanSubjectDoesNotExist_ShouldThrowStudyPlanSubjectNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanSubjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId,
                studyPlanSubjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync((StudyPlanSubject?)null);

        var useCase = new SoftDeleteStudyPlanSubjectUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<StudyPlanSubjectNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,studyPlanSubjectId,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
            x => x.SoftDeleteStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlanSubject = new StudyPlanSubject(tenantId, studyPlanId, subjectId, 3, 6, true);

        var studyPlanSubjectId = studyPlanSubject.Id;

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId,
                studyPlanSubjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlanSubject);

        dataStore
            .Setup(x => x.SoftDeleteStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new SoftDeleteStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,studyPlanSubjectId,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.SoftDeleteStudyPlanSubjectWithOutboxAsync(
                studyPlanSubject,
                It.Is<StudyPlanSubjectDeletedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.StudyPlanSubjectId == studyPlanSubjectId &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.SubjectId == subjectId &&
                    !integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}