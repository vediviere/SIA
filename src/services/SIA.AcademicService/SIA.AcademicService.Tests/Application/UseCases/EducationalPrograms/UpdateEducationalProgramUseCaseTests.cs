using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Contracts.Requests.EducationalProgramsRequest;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.EducationalPrograms;

public class UpdateEducationalProgramsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateEducationalProgram()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var educationalProgram = new EducationalProgram(tenantId, "ING001", "Ingeniería en Sistemas", "Licenciatura");

        var educationalProgramId = educationalProgram.Id;

        var request = new UpdateEducationalProgramsRequest
        {
            Code = "ING002",
            Name = "Ingeniería de Software",
            Level = "Licenciatura"
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                educationalProgramId,
                It.IsAny<CancellationToken>())).ReturnsAsync(educationalProgram);

        dataStore.Setup(x => x.UpdateEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateEducationalProgramsUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId, educationalProgramId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.Equal("ING002", educationalProgram.Code);
        Assert.Equal("Ingeniería de Software", educationalProgram.Name);
        Assert.Equal("Licenciatura", educationalProgram.Level);
        Assert.True(educationalProgram.Status);
        Assert.NotNull(educationalProgram.UpdatedAtUtc);

        Assert.Equal(educationalProgramId, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("ING002", response.Code);
        Assert.Equal("Ingeniería de Software", response.Name);
        Assert.Equal("Licenciatura", response.Level);
        Assert.True(response.Status);
        Assert.NotEqual(default, response.UpdatedAtUtc);

        dataStore.Verify(
            x => x.UpdateEducationalProgramWithOutboxAsync(
                educationalProgram,
                It.IsAny<EducationalProgramUpdatedIntegrationEvent>(),
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

        var request = new UpdateEducationalProgramsRequest
        {
            Code = "ING002",
            Name = "Ingeniería de Software",
            Level = "Licenciatura"
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                educationalProgramId,
                It.IsAny<CancellationToken>())).ReturnsAsync((EducationalProgram?)null);

        var useCase = new UpdateEducationalProgramsUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EducationalProgramNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId, educationalProgramId, request, correlationId, CancellationToken.None));

        dataStore.Verify(
            x => x.UpdateEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateEducationalProgramAndSaveOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var educationalProgram = new EducationalProgram(tenantId, "ING001", "Ingeniería en Sistemas", "Licenciatura");

        var educationalProgramId = educationalProgram.Id;

        var request = new UpdateEducationalProgramsRequest
        {
            Code = "ING002",
            Name = "Ingeniería de Software",
            Level = "Maestría"
        };

        var dataStore = new Mock<IEducationalProgramDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                tenantId,
                educationalProgramId,
                It.IsAny<CancellationToken>())).ReturnsAsync(educationalProgram);

        dataStore.Setup(x => x.UpdateEducationalProgramWithOutboxAsync(
                It.IsAny<EducationalProgram>(),
                It.IsAny<EducationalProgramUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateEducationalProgramsUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, educationalProgramId, request, correlationId, CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.UpdateEducationalProgramWithOutboxAsync(
                It.Is<EducationalProgram>(entity =>
                    entity.Id == educationalProgramId &&
                    entity.TenantId == tenantId &&
                    entity.Code == "ING002" &&
                    entity.Name == "Ingeniería de Software" &&
                    entity.Level == "Maestría" &&
                    entity.Status &&
                    entity.UpdatedAtUtc != null),
                It.Is<EducationalProgramUpdatedIntegrationEvent>(integrationEvent =>
                    integrationEvent.EventId != Guid.Empty &&
                    integrationEvent.CorrelationId == correlationId &&
                    integrationEvent.OccurredAtUtc != default &&
                    integrationEvent.TenantId == tenantId &&
                    integrationEvent.EducationalProgramId == educationalProgramId &&
                    integrationEvent.Code == "ING002" &&
                    integrationEvent.Name == "Ingeniería de Software" &&
                    integrationEvent.Level == "Maestría" &&
                    integrationEvent.Status &&
                    integrationEvent.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}