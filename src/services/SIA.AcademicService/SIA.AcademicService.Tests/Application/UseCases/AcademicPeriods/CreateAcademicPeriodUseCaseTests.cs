using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class CreateAcademicPeriodsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateAcademicPeriod()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateAcademicPeriodRequest
        {
            TenantId = tenantId,
            Code = "2026-1",
            Name = "Periodo Enero-Junio 2026",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 6, 30),
            AcademicLoadProcessStartDate = new DateOnly(2025, 11, 1),
            AcademicLoadProcessEndDate = new DateOnly(2025, 12, 15),
            EnrollmentProcessStartDate = new DateOnly(2025, 12, 1),
            EnrollmentProcessEndDate = new DateOnly(2026, 1, 15),
            PlanningSubmissionDate = new DateOnly(2025, 11, 15),
            FirstPartialGradeReportDate = new DateOnly(2026, 3, 15),
            SecondPartialGradeReportDate = new DateOnly(2026, 4, 15),
            ThirdPartialGradeReportDate = new DateOnly(2026, 5, 15),
            FinalMinutesSubmissionDate = new DateOnly(2026, 7, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        dataStore.Setup(x => x.AddAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateAcademicPeriodsUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("2026-1", response.Code);
        Assert.Equal(request.Name, response.Name);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);

        dataStore.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCodeContainingSpacesAndLowercase_ShouldNormalizeCode()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateAcademicPeriodRequest
        {
            TenantId = tenantId,
            Code = "  periodo-2026-a  ",
            Name = "Periodo Enero-Junio 2026",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 6, 30),
            AcademicLoadProcessStartDate = new DateOnly(2025, 11, 1),
            AcademicLoadProcessEndDate = new DateOnly(2025, 12, 15),
            EnrollmentProcessStartDate = new DateOnly(2025, 12, 1),
            EnrollmentProcessEndDate = new DateOnly(2026, 1, 15),
            PlanningSubmissionDate = new DateOnly(2025, 11, 15),
            FirstPartialGradeReportDate = new DateOnly(2026, 3, 15),
            SecondPartialGradeReportDate = new DateOnly(2026, 4, 15),
            ThirdPartialGradeReportDate = new DateOnly(2026, 5, 15),
            FinalMinutesSubmissionDate = new DateOnly(2026, 7, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "PERIODO-2026-A",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.AddAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new CreateAcademicPeriodsUseCase(dataStore.Object);

        // Act
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        Assert.Equal("PERIODO-2026-A", response.Code);

        dataStore.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "PERIODO-2026-A",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeAlreadyExists_ShouldThrowDuplicateAcademicPeriodCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateAcademicPeriodRequest
        {
            TenantId = tenantId,
            Code = "2026-1",
            Name = "Periodo Enero-Junio 2026",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 6, 30),
            AcademicLoadProcessStartDate = new DateOnly(2025, 11, 1),
            AcademicLoadProcessEndDate = new DateOnly(2025, 12, 15),
            EnrollmentProcessStartDate = new DateOnly(2025, 12, 1),
            EnrollmentProcessEndDate = new DateOnly(2026, 1, 15),
            PlanningSubmissionDate = new DateOnly(2025, 11, 15),
            FirstPartialGradeReportDate = new DateOnly(2026, 3, 15),
            SecondPartialGradeReportDate = new DateOnly(2026, 4, 15),
            ThirdPartialGradeReportDate = new DateOnly(2026, 5, 15),
            FinalMinutesSubmissionDate = new DateOnly(2026, 7, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-1",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateAcademicPeriodsUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateAcademicPeriodCodeException>(
            () => useCase.ExecuteAsync(request, correlationId, CancellationToken.None)
        );

        dataStore.Verify(
            x => x.AddAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveAcademicPeriodWithOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateAcademicPeriodRequest
        {
            TenantId = tenantId,
            Code = "2026-1",
            Name = "Periodo Enero-Junio 2026",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 6, 30),
            AcademicLoadProcessStartDate = new DateOnly(2025, 11, 1),
            AcademicLoadProcessEndDate = new DateOnly(2025, 12, 15),
            EnrollmentProcessStartDate = new DateOnly(2025, 12, 1),
            EnrollmentProcessEndDate = new DateOnly(2026, 1, 15),
            PlanningSubmissionDate = new DateOnly(2025, 11, 15),
            FirstPartialGradeReportDate = new DateOnly(2026, 3, 15),
            SecondPartialGradeReportDate = new DateOnly(2026, 4, 15),
            ThirdPartialGradeReportDate = new DateOnly(2026, 5, 15),
            FinalMinutesSubmissionDate = new DateOnly(2026, 7, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        dataStore.Setup(x => x.AddAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateAcademicPeriodsUseCase(dataStore.Object);

        // Act
        await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        dataStore.Verify(
            x => x.AddAcademicPeriodWithOutboxAsync(
                It.Is<AcademicPeriod>(x =>
                    x.TenantId == tenantId &&
                    x.Code == "2026-1" &&
                    x.Name == request.Name &&
                    x.StartDate == request.StartDate &&
                    x.EndDate == request.EndDate &&
                    x.Status),
                It.Is<AcademicPeriodCreatedIntegrationEvent>(e =>
                    e.CorrelationId == correlationId &&
                    e.TenantId == tenantId &&
                    e.Code == "2026-1" &&
                    e.Name == request.Name &&
                    e.StartDate == request.StartDate &&
                    e.EndDate == request.EndDate &&
                    e.Status &&
                    e.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}