using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.StudyPlans;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Contracts.Requests.StudyPlans;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.StudyPlans;

public class CreateStudyPlanUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateStudyPlan()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanRequest
        {
            TenantId = tenantId,
            EducationalProgramId = educationalProgramId,
            Code = "PLAN-2026",
            Name = "Plan de Estudios 2026",
            Version = "1.0",
            EffectiveFrom = new DateOnly(2026, 8, 1)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.StudyPlanCodeExistsAsync(
                tenantId,
                "PLAN-2026",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateStudyPlanUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(educationalProgramId, response.EducationalProgramId);
        Assert.Equal("PLAN-2026", response.Code);
        Assert.Equal("Plan de Estudios 2026", response.Name);
        Assert.Equal("1.0", response.Version);
        Assert.Equal(new DateOnly(2026, 8, 1), response.EffectiveFrom);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.CreatedAtUtc);
    }

    [Fact]
    public async Task ExecuteAsync_WithCodeContainingSpacesAndLowercase_ShouldNormalizeCode()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanRequest
        {
            TenantId = tenantId,
            EducationalProgramId = educationalProgramId,
            Code = "  plan-2026  ",
            Name = "Plan de Estudios 2026",
            Version = "1.0",
            EffectiveFrom = new DateOnly(2026, 8, 1)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.StudyPlanCodeExistsAsync(
                tenantId,
                "PLAN-2026",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore
            .Setup(x => x.AddStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateStudyPlanUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        Assert.Equal("PLAN-2026", response.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateStudyPlanCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanRequest
        {
            TenantId = tenantId,
            EducationalProgramId = educationalProgramId,
            Code = "PLAN-2026",
            Name = "Plan de Estudios 2026",
            Version = "1.0",
            EffectiveFrom = new DateOnly(2026, 8, 1)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.StudyPlanCodeExistsAsync(
                tenantId,
                "PLAN-2026",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateStudyPlanUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateStudyPlanCodeException>(() =>
            useCase.ExecuteAsync(
                request,
                correlationId,
                CancellationToken.None));

        // Assert
        dataStore.Verify(
            x => x.AddStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveStudyPlanWithOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateStudyPlanRequest
        {
            TenantId = tenantId,
            EducationalProgramId = educationalProgramId,
            Code = "PLAN-2026",
            Name = "Plan de Estudios 2026",
            Version = "1.0",
            EffectiveFrom = new DateOnly(2026, 8, 1)
        };

        var dataStore = new Mock<IStudyPlanDataStore>();

        dataStore.Setup(x => x.StudyPlanCodeExistsAsync(
                tenantId,
                "PLAN-2026",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddStudyPlanWithOutboxAsync(
                It.IsAny<StudyPlan>(),
                It.IsAny<StudyPlanCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateStudyPlanUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.AddStudyPlanWithOutboxAsync(
                It.Is<StudyPlan>(studyPlan =>
                    studyPlan.TenantId == tenantId &&
                    studyPlan.EducationalProgramId == educationalProgramId &&
                    studyPlan.Code == "PLAN-2026" &&
                    studyPlan.Name == "Plan de Estudios 2026" &&
                    studyPlan.Version == "1.0" &&
                    studyPlan.EffectiveFrom == new DateOnly(2026, 8, 1) &&
                    studyPlan.Status),
                It.Is<StudyPlanCreatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.EducationalProgramId == educationalProgramId &&
                    integrationEvent.Code == "PLAN-2026" &&
                    integrationEvent.Name == "Plan de Estudios 2026" &&
                    integrationEvent.Version == "1.0" &&
                    integrationEvent.EffectiveFrom == new DateOnly(2026, 8, 1) &&
                    integrationEvent.Status &&
                    integrationEvent.ContractVersion == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}