using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.EducationalPrograms;

public class CreateEducationalProgramsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateEducationalProgram()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateEducationalProgramsRequest
        {
            TenantId = tenantId,
            Code = "  ing001  ",
            Name = " Ingeniería en Sistemas ",
            Level = " Licenciatura "
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING001",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateEducationalProgramsUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("ING001", response.Code);
        Assert.Equal("Ingeniería en Sistemas", response.Name);
        Assert.Equal("Licenciatura", response.Level);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.CreatedAtUtc);

        dataStore.Verify(
            x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING001",
                It.IsAny<CancellationToken>()),
            Times.Once);

        dataStore.Verify(
            x => x.AddEducationalProgramWithOutboxAsync(
                It.Is<EducationalProgram>(entity =>
                    entity.Id == response.Id &&
                    entity.TenantId == tenantId &&
                    entity.Code == "ING001" &&
                    entity.Name == "Ingeniería en Sistemas" &&
                    entity.Level == "Licenciatura" &&
                    entity.Status &&
                    entity.CreatedAtUtc == response.CreatedAtUtc),
                It.Is<EducationalProgramCreatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.EducationalProgramId == response.Id &&
                    integrationEvent.Code == "ING001" &&
                    integrationEvent.Name == "Ingeniería en Sistemas" &&
                    integrationEvent.Level == "Licenciatura" &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCodeContainingSpacesAndLowercase_ShouldNormalizeCode()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateEducationalProgramsRequest
        {
            TenantId = tenantId,
            Code = "  ing-2026  ",
            Name = "Ingeniería",
            Level = "Licenciatura"
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING-2026",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateEducationalProgramsUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        Assert.Equal("ING-2026", response.Code);

        dataStore.Verify(
            x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING-2026",
                It.IsAny<CancellationToken>()),
            Times.Once);

        dataStore.Verify(
            x => x.AddEducationalProgramWithOutboxAsync(
                It.Is<EducationalProgram>(entity =>
                    entity.Code == "ING-2026"),
                It.Is<EducationalProgramCreatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.Code == "ING-2026"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateEducationalProgramCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateEducationalProgramsRequest
        {
            TenantId = tenantId,
            Code = "  ing001  ",
            Name = "Ingeniería en Sistemas",
            Level = "Licenciatura"
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING001",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateEducationalProgramsUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEducationalProgramCodeException>(() =>
            useCase.ExecuteAsync(request,correlationId,CancellationToken.None)
        );

        dataStore.Verify(
            x => x.EducationalProgramCodeExistsAsync(
                tenantId,
                "ING001",
                It.IsAny<CancellationToken>()),
            Times.Once);

        dataStore.Verify(
            x => x.AddEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}