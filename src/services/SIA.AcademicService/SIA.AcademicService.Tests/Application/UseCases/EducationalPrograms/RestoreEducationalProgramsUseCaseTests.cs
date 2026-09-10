using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.EducationalPrograms;

public class RestoreEducationalProgramsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingEducationalProgram_ShouldRestoreAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var educationalProgram = new EducationalProgram(tenantId, "ING001", "Ingeniería en Sistemas", "Licenciatura");

        var educationalProgramId = educationalProgram.Id;

        // Dejamos la entidad desactivada antes de ejecutar el UseCase.
        educationalProgram.Desactivate();

        Assert.False(educationalProgram.Status);
        Assert.NotNull(educationalProgram.UpdatedAtUtc);

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                educationalProgramId,
                It.IsAny<CancellationToken>())).ReturnsAsync(educationalProgram);

        dataStore.Setup(x => x.RestoreEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new RestoreEducationalProgramsUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, educationalProgramId, correlationId, CancellationToken.None);

        // Assert
        Assert.True(educationalProgram.Status);
        Assert.NotNull(educationalProgram.UpdatedAtUtc);

        dataStore.Verify(
            x => x.RestoreEducationalProgramWithOutboxAsync(
                educationalProgram,
                It.Is<EducationalProgramRestoredIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.EducationalProgramId == educationalProgramId &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEducationalProgramDoesNotExist_ShouldThrowEducationalProgramNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var educationalProgramId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                educationalProgramId,
                It.IsAny<CancellationToken>())).ReturnsAsync((EducationalProgram?)null);

        var useCase = new RestoreEducationalProgramsUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EducationalProgramNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId, educationalProgramId, correlationId, CancellationToken.None));

        dataStore.Verify(
            x => x.RestoreEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}