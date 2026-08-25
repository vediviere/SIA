using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.ServiceComplementaries;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.ServiceComplementaries;

public class RestoreServiceComplementaryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldRestoreServiceComplementary()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceComplementary = new ServiceComplementary(tenantId, studyPlanId, true, 4);

        // Dejamos la entidad desactivada para representar
        // el estado previo a la restauración.
        serviceComplementary.SoftDelete();

        var serviceComplementaryId = serviceComplementary.Id;

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync(serviceComplementary);

        dataStore.Setup(x => x.RestoreServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new RestoreServiceComplementaryUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,serviceComplementaryId,correlationId,CancellationToken.None);

        // Assert
        Assert.True(serviceComplementary.Status);
        Assert.NotNull(serviceComplementary.UpdatedAtUtc);

        dataStore.Verify(
            x => x.RestoreServiceComplementaryWithOutboxAsync(
                serviceComplementary,
                It.IsAny<ServiceComplementaryRestoredIntegrationEvent>(),
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

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync((ServiceComplementary?)null);

        var useCase = new RestoreServiceComplementaryUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceComplementaryNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,serviceComplementaryId,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
            x => x.RestoreServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var studyPlanId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceComplementary = new ServiceComplementary(tenantId, studyPlanId, true, 4);

        serviceComplementary.SoftDelete();

        var serviceComplementaryId = serviceComplementary.Id;

        var dataStore = new Mock<IServiceComplementaryDataStore>();

        dataStore.Setup(x => x.GetServiceComplementaryByIdAsync(
                tenantId,
                serviceComplementaryId,
                It.IsAny<CancellationToken>())).ReturnsAsync(serviceComplementary);

        dataStore.Setup(x => x.RestoreServiceComplementaryWithOutboxAsync(
                It.IsAny<ServiceComplementary>(),
                It.IsAny<ServiceComplementaryRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new RestoreServiceComplementaryUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,serviceComplementaryId,correlationId,CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.RestoreServiceComplementaryWithOutboxAsync(
                serviceComplementary,
                It.Is<ServiceComplementaryRestoredIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.ServiceComplementaryId == serviceComplementaryId &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}