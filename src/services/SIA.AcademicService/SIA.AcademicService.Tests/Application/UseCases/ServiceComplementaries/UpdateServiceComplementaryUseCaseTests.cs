using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.ServiceComplementaries;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Contracts.Requests.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.ServiceComplementaries;

public class UpdateServiceComplementaryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateServiceComplementary()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceComplementary = new ServiceComplementary(tenantId, studyPlanId, true, 4);

        var serviceComplementaryId = serviceComplementary.Id;

        var request = new UpdateServiceComplementaryRequest
        {
            Type = false,
            Credit = 8
        };

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync(serviceComplementary);

        dataStore
            .Setup(x => x.UpdateServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new UpdateServiceComplementaryUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId, serviceComplementaryId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.False(serviceComplementary.Type);
        Assert.Equal(8, serviceComplementary.Credit);
        Assert.NotNull(serviceComplementary.UpdatedAtUtc);

        Assert.Equal(serviceComplementaryId, response.Id);
        Assert.False(response.Type);
        Assert.Equal(8, response.Credit);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);

        dataStore.Verify(
            x => x.UpdateServiceComplementaryWithOutboxAsync(
                serviceComplementary,
                It.IsAny<ServiceComplementaryUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceComplementaryDoesNotExist_ShouldThrowServiceComplementaryNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var serviceComplementaryId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new UpdateServiceComplementaryRequest
        {
            Type = false,
            Credit = 8
        };

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync((ServiceComplementary?)null);

        var useCase = new UpdateServiceComplementaryUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceComplementaryNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,serviceComplementaryId,request,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
            x => x.UpdateServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateServiceComplementaryAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceComplementary = new ServiceComplementary(tenantId, studyPlanId, true, 4);

        var serviceComplementaryId = serviceComplementary.Id;

        var request = new UpdateServiceComplementaryRequest
        {
            Type = false,
            Credit = 8
        };

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync(serviceComplementary);

        dataStore.Setup(x => x.UpdateServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateServiceComplementaryUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, serviceComplementaryId, request, correlationId, CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.UpdateServiceComplementaryWithOutboxAsync(
                It.Is<ServiceComplementary>(entity =>
                    entity.Id == serviceComplementaryId &&
                    entity.TenantId == tenantId &&
                    entity.StudyPlanId == studyPlanId &&
                    !entity.Type &&
                    entity.Credit == 8 &&
                    entity.Status &&
                    entity.UpdatedAtUtc != null),
                It.Is<ServiceComplementaryUpdatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.ServiceComplementaryId == serviceComplementaryId &&
                    !integrationEvent.Type &&
                    integrationEvent.Credit == 8 &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}