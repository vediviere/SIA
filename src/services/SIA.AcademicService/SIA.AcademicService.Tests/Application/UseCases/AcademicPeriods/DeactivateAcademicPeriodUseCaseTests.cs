using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class DeactivateAcademicPeriodUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidAcademicPeriod_ShouldDeactivateAcademicPeriod()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = CreateAcademicPeriod(tenantId, status: true);

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriod.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        dataStore.Setup(x => x.DeactivateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodDeactivatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new DeactivateAcademicPeriodUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId, academicPeriod.Id, correlationId, CancellationToken.None);

        // Assert
        Assert.False(academicPeriod.Status);
        Assert.False(response.Status);
        Assert.Equal(academicPeriod.Id, response.Id);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.NotNull(response.UpdatedAtUtc);

        dataStore.Verify(
            x => x.GetByIdAsync(
                academicPeriod.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAcademicPeriodDoesNotExist_ShouldThrowAcademicPeriodNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync((AcademicPeriod?)null);

        var useCase = new DeactivateAcademicPeriodUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(
            () => useCase.ExecuteAsync(tenantId, academicPeriodId, correlationId, CancellationToken.None)
        );

        dataStore.Verify(
            x => x.DeactivateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodDeactivatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidAcademicPeriod_ShouldDeactivateAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = CreateAcademicPeriod(tenantId, status: true);

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriod.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        dataStore.Setup(x => x.DeactivateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodDeactivatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new DeactivateAcademicPeriodUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, academicPeriod.Id, correlationId, CancellationToken.None);

        // Assert
        Assert.False(academicPeriod.Status);
        Assert.NotNull(academicPeriod.UpdatedAtUtc);

        dataStore.Verify(
            x => x.DeactivateAcademicPeriodWithOutboxAsync(
                It.Is<AcademicPeriod>(x =>
                    x.Id == academicPeriod.Id &&
                    x.TenantId == tenantId &&
                    !x.Status &&
                    x.UpdatedAtUtc.HasValue),
                It.Is<AcademicPeriodDeactivatedIntegrationEvent>(e =>
                    e.EventId != Guid.Empty &&
                    e.CorrelationId == correlationId &&
                    e.TenantId == tenantId &&
                    e.AcademicPeriodId == academicPeriod.Id &&
                    !e.Status &&
                    e.OccurredAtUtc == academicPeriod.UpdatedAtUtc!.Value &&
                    e.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static AcademicPeriod CreateAcademicPeriod(Guid tenantId,bool status)
    {
        var academicPeriod = new AcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo Enero-Junio 2026",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            new DateOnly(2025, 11, 1),
            new DateOnly(2025, 12, 15),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2025, 11, 15),
            new DateOnly(2026, 3, 15),
            new DateOnly(2026, 4, 15),
            new DateOnly(2026, 5, 15),
            new DateOnly(2026, 7, 15));

        if (!status)
        {
            academicPeriod.Deactivate();
        }

        return academicPeriod;
    }
}