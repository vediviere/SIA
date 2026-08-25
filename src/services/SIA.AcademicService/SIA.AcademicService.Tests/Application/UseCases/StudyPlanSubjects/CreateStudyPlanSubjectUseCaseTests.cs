using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlanSubjects;

public class CreateStudyPlanSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateStudyPlanSubject()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanSubjectRequest
        {
            TenantId = tenantId,
            StudyPlanId = studyPlanId,
            SubjectId = subjectId,
            Semester = 3,
            Credits = 6,
            IsRequired = true
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId,
                studyPlanId,
                subjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(studyPlanId, response.StudyPlanId);
        Assert.Equal(subjectId, response.SubjectId);
        Assert.Equal(3, response.Semester);
        Assert.Equal(6, response.Credits);
        Assert.True(response.IsRequired);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.CreatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);

        dataStore.Verify(
            x => x.AddStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationAlreadyExists_ShouldThrowDuplicateStudyPlanSubjectException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanSubjectRequest
        {
            TenantId = tenantId,
            StudyPlanId = studyPlanId,
            SubjectId = subjectId,
            Semester = 3,
            Credits = 6,
            IsRequired = true
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId,
                studyPlanId,
                subjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateStudyPlanSubjectUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateStudyPlanSubjectException>(() =>
            useCase.ExecuteAsync(request,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
            x => x.AddStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveStudyPlanSubjectWithOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanSubjectRequest
        {
            TenantId = tenantId,
            StudyPlanId = studyPlanId,
            SubjectId = subjectId,
            Semester = 3,
            Credits = 6,
            IsRequired = true
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId,
                studyPlanId,
                subjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.AddStudyPlanSubjectWithOutboxAsync(
                It.Is<StudyPlanSubject>(entity =>
                    entity.TenantId == tenantId &&
                    entity.StudyPlanId == studyPlanId &&
                    entity.SubjectId == subjectId &&
                    entity.Semester == 3 &&
                    entity.Credits == 6 &&
                    entity.IsRequired &&
                    entity.Status &&
                    entity.Id != Guid.Empty &&
                    entity.CreatedAtUtc != default),
                It.Is<StudyPlanSubjectCreatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.StudyPlanSubjectId != Guid.Empty &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.SubjectId == subjectId &&
                    integrationEvent.Semester == 3 &&
                    integrationEvent.Credits == 6 &&
                    integrationEvent.IsRequired &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}