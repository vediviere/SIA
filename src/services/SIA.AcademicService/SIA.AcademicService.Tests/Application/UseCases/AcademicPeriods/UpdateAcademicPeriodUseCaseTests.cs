using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class UpdateAcademicPeriodUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateAcademicPeriod()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = new AcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo Académico 2026-1",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 1, 31),
            new DateOnly(2026, 1, 20),
            new DateOnly(2026, 3, 31),
            new DateOnly(2026, 4, 30),
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 7, 15));

        var academicPeriodId = academicPeriod.Id;

        var request = new UpdateAcademicPeriodRequest
        {
            Code = "2026-2",
            Name = "Periodo Académico 2026-2",
            StartDate = new DateOnly(2026, 8, 1),
            EndDate = new DateOnly(2026, 12, 31),
            AcademicLoadProcessStartDate = new DateOnly(2026, 7, 1),
            AcademicLoadProcessEndDate = new DateOnly(2026, 8, 15),
            EnrollmentProcessStartDate = new DateOnly(2026, 7, 15),
            EnrollmentProcessEndDate = new DateOnly(2026, 8, 31),
            PlanningSubmissionDate = new DateOnly(2026, 8, 10),
            FirstPartialGradeReportDate = new DateOnly(2026, 9, 30),
            SecondPartialGradeReportDate = new DateOnly(2026, 10, 31),
            ThirdPartialGradeReportDate = new DateOnly(2026, 11, 30),
            FinalMinutesSubmissionDate = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateAcademicPeriodUseCase(dataStore.Object);

        // Act
        var result = await useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(academicPeriodId, result.Id);
        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal("2026-2", result.Code);
        Assert.Equal("Periodo Académico 2026-2", result.Name);
        Assert.Equal(new DateOnly(2026, 8, 1), result.StartDate);
        Assert.Equal(new DateOnly(2026, 12, 31), result.EndDate);
        Assert.True(result.UpdatedAtUtc.HasValue);
        Assert.Equal(correlationId, result.CorrelationId);

        Assert.Equal("2026-2", academicPeriod.Code);
        Assert.Equal("Periodo Académico 2026-2", academicPeriod.Name);
        Assert.Equal(new DateOnly(2026, 8, 1), academicPeriod.StartDate);
        Assert.Equal(new DateOnly(2026, 12, 31), academicPeriod.EndDate);

        dataStore.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                academicPeriod,
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
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

        var request = new UpdateAcademicPeriodRequest
        {
            Code = "2026-2",
            Name = "Periodo Académico 2026-2",
            StartDate = new DateOnly(2026, 8, 1),
            EndDate = new DateOnly(2026, 12, 31),
            AcademicLoadProcessStartDate = new DateOnly(2026, 7, 1),
            AcademicLoadProcessEndDate = new DateOnly(2026, 8, 15),
            EnrollmentProcessStartDate = new DateOnly(2026, 7, 15),
            EnrollmentProcessEndDate = new DateOnly(2026, 8, 31),
            PlanningSubmissionDate = new DateOnly(2026, 8, 10),
            FirstPartialGradeReportDate = new DateOnly(2026, 9, 30),
            SecondPartialGradeReportDate = new DateOnly(2026, 10, 31),
            ThirdPartialGradeReportDate = new DateOnly(2026, 11, 30),
            FinalMinutesSubmissionDate = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync((AcademicPeriod?)null);

        var useCase = new UpdateAcademicPeriodUseCase(dataStore.Object);

        // Act & Assert
        await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None)  
        );

        dataStore.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ExecuteAsync_WhenNewCodeAlreadyExists_ShouldThrowDuplicateAcademicPeriodCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = new AcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo Académico 2026-1",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 1, 31),
            new DateOnly(2026, 1, 20),
            new DateOnly(2026, 3, 31),
            new DateOnly(2026, 4, 30),
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 7, 15));

        var academicPeriodId = academicPeriod.Id;

        var request = new UpdateAcademicPeriodRequest
        {
            Code = "  2026-2  ",
            Name = "Periodo Académico 2026-2",
            StartDate = new DateOnly(2026, 8, 1),
            EndDate = new DateOnly(2026, 12, 31),
            AcademicLoadProcessStartDate = new DateOnly(2026, 7, 1),
            AcademicLoadProcessEndDate = new DateOnly(2026, 8, 15),
            EnrollmentProcessStartDate = new DateOnly(2026, 7, 15),
            EnrollmentProcessEndDate = new DateOnly(2026, 8, 31),
            PlanningSubmissionDate = new DateOnly(2026, 8, 10),
            FirstPartialGradeReportDate = new DateOnly(2026, 9, 30),
            SecondPartialGradeReportDate = new DateOnly(2026, 10, 31),
            ThirdPartialGradeReportDate = new DateOnly(2026, 11, 30),
            FinalMinutesSubmissionDate = new DateOnly(2027, 1, 15)
        };

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new UpdateAcademicPeriodUseCase(dataStore.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DuplicateAcademicPeriodCodeException>(() =>
            useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None)
        );

        Assert.NotNull(exception);

        dataStore.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>()),
            Times.Once);

        dataStore.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldSaveUpdatedAcademicPeriodWithOutboxEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = new AcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo Académico 2026-1",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 1, 31),
            new DateOnly(2026, 1, 20),
            new DateOnly(2026, 3, 31),
            new DateOnly(2026, 4, 30),
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 7, 15));

        var academicPeriodId = academicPeriod.Id;

        var request = new UpdateAcademicPeriodRequest
        {
            Code = "2026-2",
            Name = "Periodo Académico 2026-2",
            StartDate = new DateOnly(2026, 8, 1),
            EndDate = new DateOnly(2026, 12, 31),
            AcademicLoadProcessStartDate = new DateOnly(2026, 7, 1),
            AcademicLoadProcessEndDate = new DateOnly(2026, 8, 15),
            EnrollmentProcessStartDate = new DateOnly(2026, 7, 15),
            EnrollmentProcessEndDate = new DateOnly(2026, 8, 31),
            PlanningSubmissionDate = new DateOnly(2026, 8, 10),
            FirstPartialGradeReportDate = new DateOnly(2026, 9, 30),
            SecondPartialGradeReportDate = new DateOnly(2026, 10, 31),
            ThirdPartialGradeReportDate = new DateOnly(2026, 11, 30),
            FinalMinutesSubmissionDate = new DateOnly(2027, 1, 15)
        };

        AcademicPeriodUpdatedIntegrationEvent? capturedEvent = null;

        var dataStore = new Mock<IAcademicPeriodsDataStore>();

        dataStore.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        dataStore.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        dataStore.Setup(x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Callback<AcademicPeriod, AcademicPeriodUpdatedIntegrationEvent, CancellationToken>(
                (_, integrationEvent, _) =>
                {
                    capturedEvent = integrationEvent;
                })
            .Returns(Task.CompletedTask);

        var useCase = new UpdateAcademicPeriodUseCase(dataStore.Object);

        // Act
        var result = await useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEvent);

        Assert.Equal(correlationId, capturedEvent!.CorrelationId);
        Assert.Equal(tenantId, capturedEvent.TenantId);
        Assert.Equal(academicPeriodId, capturedEvent.AcademicPeriodId);
        Assert.Equal("2026-2", capturedEvent.Code);
        Assert.Equal("Periodo Académico 2026-2", capturedEvent.Name);
        Assert.Equal(new DateOnly(2026, 8, 1), capturedEvent.StartDate);
        Assert.Equal(new DateOnly(2026, 12, 31), capturedEvent.EndDate);
        Assert.Equal(academicPeriod.Status, capturedEvent.Status);
        Assert.Equal(1, capturedEvent.Version);

        Assert.Equal(academicPeriod.UpdatedAtUtc, capturedEvent.OccurredAtUtc);

        dataStore.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                academicPeriod,
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal(correlationId, result.CorrelationId);
    }
}