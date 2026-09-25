using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.Requests.StudyPlanSubjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlanSubjects;

public class UpdateStudyPlanSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var prerequisiteId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlanSubject = new StudyPlanSubject(tenantId, studyPlanId, subjectId, 2, 4, false, null);

        var studyPlanSubjectId = studyPlanSubject.Id;

        var request = new UpdateStudyPlanSubjectRequest
        {
            Semester = 5,
            Credits = 8,
            IsRequired = true,
            PrerequisiteSubjectId = prerequisiteId
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId, studyPlanSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studyPlanSubject); 

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId, studyPlanId, prerequisiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        dataStore.Setup(x => x.UpdateStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask); 

        var useCase = new UpdateStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId, studyPlanSubjectId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.Equal(5, studyPlanSubject.Semester);
        Assert.Equal(8, studyPlanSubject.Credits);
        Assert.True(studyPlanSubject.IsRequired);
        Assert.Equal(prerequisiteId, studyPlanSubject.PrerequisiteSubjectId);
        Assert.NotNull(studyPlanSubject.UpdatedAtUtc);

        Assert.Equal(studyPlanSubjectId, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(studyPlanId, response.StudyPlanId);
        Assert.Equal(subjectId, response.SubjectId);
        Assert.Equal(prerequisiteId, response.PrerequisiteSubjectId);
        Assert.Equal(5, response.Semester);
        Assert.Equal(8, response.Credits);
        Assert.True(response.IsRequired);
        Assert.True(response.Status);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);

        dataStore.Verify(
            x => x.UpdateStudyPlanSubjectWithOutboxAsync(
                studyPlanSubject,
                It.IsAny<StudyPlanSubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEntityDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanSubjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new UpdateStudyPlanSubjectRequest
        {
            Semester = 5,
            Credits = 8,
            IsRequired = true,
            PrerequisiteSubjectId = Guid.NewGuid()
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId, studyPlanSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudyPlanSubject?)null); 

        var useCase = new UpdateStudyPlanSubjectUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<StudyPlanSubjectNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId, studyPlanSubjectId, request, correlationId, CancellationToken.None));

        dataStore.Verify(
            x => x.UpdateStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never); 
    }

    [Fact]
    public async Task ExecuteAsync_WhenPrerequisiteDoesNotExistInPlan_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var prerequisiteId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlanSubject = new StudyPlanSubject(tenantId, studyPlanId, subjectId, 2, 4, false, null);
        var studyPlanSubjectId = studyPlanSubject.Id;

        var request = new UpdateStudyPlanSubjectRequest
        {
            Semester = 5,
            Credits = 8,
            IsRequired = true,
            PrerequisiteSubjectId = prerequisiteId
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId, studyPlanSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studyPlanSubject); 

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId, studyPlanId, prerequisiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var useCase = new UpdateStudyPlanSubjectUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(tenantId, studyPlanSubjectId, request, correlationId, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var prerequisiteId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlanSubject = new StudyPlanSubject(tenantId, studyPlanId, subjectId, 2, 4, false, null);

        var studyPlanSubjectId = studyPlanSubject.Id;

        var request = new UpdateStudyPlanSubjectRequest
        {
            Semester = 5,
            Credits = 8,
            IsRequired = true,
            PrerequisiteSubjectId = prerequisiteId
        };

        var dataStore = new Mock<IStudyPlanSubjectDataStore>();

        dataStore.Setup(x => x.GetStudyPlanSubjectByIdAsync(
                tenantId, studyPlanSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studyPlanSubject); 

        dataStore.Setup(x => x.StudyPlanSubjectExistsAsync(
                tenantId, studyPlanId, prerequisiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        dataStore.Setup(x => x.UpdateStudyPlanSubjectWithOutboxAsync(
                It.IsAny<StudyPlanSubject>(),
                It.IsAny<StudyPlanSubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new UpdateStudyPlanSubjectUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, studyPlanSubjectId, request, correlationId, CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.UpdateStudyPlanSubjectWithOutboxAsync(
                It.Is<StudyPlanSubject>(entity =>
                    entity.Id == studyPlanSubjectId &&
                    entity.TenantId == tenantId &&
                    entity.StudyPlanId == studyPlanId &&
                    entity.SubjectId == subjectId &&
                    entity.PrerequisiteSubjectId == prerequisiteId &&
                    entity.Semester == 5 &&
                    entity.Credits == 8 &&
                    entity.IsRequired &&
                    entity.Status &&
                    entity.UpdatedAtUtc != null),
                It.Is<StudyPlanSubjectUpdatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.StudyPlanSubjectId == studyPlanSubjectId &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.SubjectId == subjectId &&
                    integrationEvent.PrerequisiteSubjectId == prerequisiteId &&
                    integrationEvent.Semester == 5 &&
                    integrationEvent.Credits == 8 &&
                    integrationEvent.IsRequired &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once); 
    }
}