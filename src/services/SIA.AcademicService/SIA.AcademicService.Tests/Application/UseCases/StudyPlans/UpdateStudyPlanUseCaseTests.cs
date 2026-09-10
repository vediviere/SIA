using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlans;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Contracts.Requests.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlans;

public class UpdateStudyPlanUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateStudyPlan()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "PLAN-2026",
            "Plan de Estudios 2026",
            "1.0",
            new DateOnly(2026, 8, 1)
        );

        var studyPlanId = studyPlan.Id;

        var request = new UpdateStudyPlanRequest
        {
            Code = "PLAN-2027",
            Name = "Plan de Estudios 2027",
            Version = "2.0",
            EffectiveFrom = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlan);

        dataStore.Setup(x => x.UpdateStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateStudyPlanUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(
            tenantId,
            studyPlanId,
            request,
            correlationId,
            CancellationToken.None);

        // Assert
        Assert.Equal(studyPlan.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(educationalProgramId, response.EducationalProgramId);
        Assert.Equal("PLAN-2027", response.Code);
        Assert.Equal("Plan de Estudios 2027", response.Name);
        Assert.Equal("2.0", response.Version);
        Assert.Equal(new DateOnly(2027, 1, 15), response.EffectiveFrom);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.UpdatedAtUtc);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudyPlanDoesNotExist_ShouldThrowStudyPlanNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new UpdateStudyPlanRequest
        {
            Code = "PLAN-2027",
            Name = "Plan de Estudios 2027",
            Version = "2.0",
            EffectiveFrom = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync((StudyPlan?)null);

        var useCase = new UpdateStudyPlanUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<StudyPlanNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,studyPlanId,request,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
        x => x.UpdateStudyPlanWithOutboxAsync(
            It.IsAny<StudyPlan>(),
            It.IsAny<StudyPlanUpdatedIntegrationEvent>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateStudyPlanAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var studyPlan = new StudyPlan(
            tenantId,
            educationalProgramId,
            "PLAN-2026",
            "Plan de Estudios 2026",
            "1.0",
            new DateOnly(2026, 8, 1));

        var studyPlanId = studyPlan.Id;

        var request = new UpdateStudyPlanRequest
        {
            Code = "PLAN-2027",
            Name = "Plan de Estudios 2027",
            Version = "2.0",
            EffectiveFrom = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                studyPlanId,
                It.IsAny<CancellationToken>())).ReturnsAsync(studyPlan);

        dataStore.Setup(x => x.UpdateStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateStudyPlanUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(
            tenantId,
            studyPlanId,
            request,
            correlationId,
            CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.UpdateStudyPlanWithOutboxAsync(
                It.Is<StudyPlan>(entity =>
                    entity.Id == studyPlanId &&
                    entity.TenantId == tenantId &&
                    entity.EducationalProgramId == educationalProgramId &&
                    entity.Code == "PLAN-2027" &&
                    entity.Name == "Plan de Estudios 2027" &&
                    entity.Version == "2.0" &&
                    entity.EffectiveFrom == new DateOnly(2027, 1, 15) &&
                    entity.Status),
                It.Is<StudyPlanUpdatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.EducationalProgramId == educationalProgramId &&
                    integrationEvent.Code == "PLAN-2027" &&
                    integrationEvent.Name == "Plan de Estudios 2027" &&
                    integrationEvent.Version == "2.0" &&
                    integrationEvent.EffectiveFrom == new DateOnly(2027, 1, 15) &&
                    integrationEvent.Status &&
                    integrationEvent.ContractVersion == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}