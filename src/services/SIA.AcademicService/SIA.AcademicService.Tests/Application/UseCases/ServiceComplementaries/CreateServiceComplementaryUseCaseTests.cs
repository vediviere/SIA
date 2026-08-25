using Moq;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.ServiceComplementaries;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Contracts.Requests.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.ServiceComplementaries;

public class CreateServiceComplementaryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateServiceComplementary()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateServiceComplementaryRequest
        {
            TenantId = tenantId,
            StudyPlanId = studyPlanId,
            Type = true,
            Credit = 4
        };

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.AddServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateServiceComplementaryUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(studyPlanId, response.StudyPlanId);
        Assert.True(response.Type);
        Assert.Equal(4, response.Credit);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.CreatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);

        dataStore.Verify(
            x => x.AddServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveServiceComplementaryWithOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateServiceComplementaryRequest
        {
            TenantId = tenantId,
            StudyPlanId = studyPlanId,
            Type = true,
            Credit = 4
        };

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.AddServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateServiceComplementaryUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(request,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.AddServiceComplementaryWithOutboxAsync(
                It.Is<ServiceComplementary>(entity =>
                    entity.Id != Guid.Empty &&
                    entity.TenantId == tenantId &&
                    entity.StudyPlanId == studyPlanId &&
                    entity.Type &&
                    entity.Credit == 4 &&
                    entity.Status &&
                    entity.CreatedAtUtc != default),
                It.Is<ServiceComplementaryCreatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.ServiceComplementaryId != Guid.Empty &&
                    integrationEvent.StudyPlanId == studyPlanId &&
                    integrationEvent.Type &&
                    integrationEvent.Credit == 4 &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}