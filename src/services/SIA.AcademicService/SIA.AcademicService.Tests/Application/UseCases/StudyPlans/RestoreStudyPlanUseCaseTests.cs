using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlans;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlans;

public class RestoreStudyPlanUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldRestoreStudyPlan()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "SP-001",
            "Plan de estudios",
            "1.0",
            new DateOnly(2026, 1, 1));

        // Dejamos la entidad desactivada para representar
        // el estado previo a la restauración.
        studyPlan.Desactivate();

        var studyPlanId = studyPlan.Id;

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlan);

        dataStore.Setup(x => x.RestoreStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new RestoreStudyPlanUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,studyPlanId,correlationId,CancellationToken.None);

        // Assert
        Assert.True(studyPlan.Status);
        Assert.NotNull(studyPlan.UpdatedAtUtc);

        dataStore.Verify(
            x => x.RestoreStudyPlanWithOutboxAsync(
                studyPlan,
                It.IsAny<StudyPlanRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudyPlanDoesNotExist_ShouldThrowStudyPlanNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync((StudyPlan?)null);

        var useCase = new RestoreStudyPlanUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<StudyPlanNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,studyPlanId,correlationId,CancellationToken.None));

        dataStore.Verify(
            x => x.RestoreStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "SP-001",
            "Plan de estudios",
            "1.0",
            new DateOnly(2026, 1, 1));

        studyPlan.Desactivate();

        var studyPlanId = studyPlan.Id;

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlan);

        dataStore.Setup(x => x.RestoreStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new RestoreStudyPlanUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,studyPlanId,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.RestoreStudyPlanWithOutboxAsync(
                studyPlan,
                It.Is<StudyPlanRestoredIntegrationEvent>(integrationEvent =>
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.ContractVersion == 1 &&
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.OccurredAtUtc != default),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}