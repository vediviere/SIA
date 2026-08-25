using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class PatchAcademicPeriodUseCaseTests
{
    private readonly Mock<IAcademicPeriodsDataStore> _dataStoreMock;
    private readonly PatchAcademicPeriodUseCase _useCase;

    public PatchAcademicPeriodUseCaseTests()
    {
        _dataStoreMock = new Mock<IAcademicPeriodsDataStore>();
        _useCase = new PatchAcademicPeriodUseCase(_dataStoreMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateAcademicPeriodAndReturnResponse_WhenAcademicPeriodExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = CreateAcademicPeriod(tenantId, "2026-1", "Periodo 2026-1");

        // Usamos el Id generado por la entidad.
        academicPeriodId = academicPeriod.Id;

        var request = CreateRequest(code: " 2026-2 ", name: "Periodo 2026-2");

        _dataStoreMock.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        _dataStoreMock.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        _dataStoreMock.Setup(x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(academicPeriod.Id, result.Id);
        Assert.Equal(academicPeriod.TenantId, result.TenantId);
        Assert.Equal("2026-2", result.Code);
        Assert.Equal("Periodo 2026-2", result.Name);
        Assert.Equal(academicPeriod.StartDate, result.StartDate);
        Assert.Equal(academicPeriod.EndDate, result.EndDate);
        Assert.Equal(academicPeriod.AcademicLoadProcessStartDate, result.AcademicLoadProcessStartDate);
        Assert.Equal(academicPeriod.AcademicLoadProcessEndDate, result.AcademicLoadProcessEndDate);
        Assert.Equal(academicPeriod.EnrollmentProcessStartDate, result.EnrollmentProcessStartDate);
        Assert.Equal(academicPeriod.EnrollmentProcessEndDate, result.EnrollmentProcessEndDate);
        Assert.Equal(academicPeriod.PlanningSubmissionDate, result.PlanningSubmissionDate);
        Assert.Equal(academicPeriod.FirstPartialGradeReportDate, result.FirstPartialGradeReportDate);
        Assert.Equal(academicPeriod.SecondPartialGradeReportDate, result.SecondPartialGradeReportDate);
        Assert.Equal(academicPeriod.ThirdPartialGradeReportDate, result.ThirdPartialGradeReportDate);
        Assert.Equal(academicPeriod.FinalMinutesSubmissionDate, result.FinalMinutesSubmissionDate);
        Assert.Equal(academicPeriod.Status, result.Status);
        Assert.Equal(academicPeriod.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(academicPeriod.UpdatedAtUtc, result.UpdatedAtUtc);
        Assert.Equal(correlationId, result.CorrelationId);

        _dataStoreMock.Verify(
            x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _dataStoreMock.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>()),
            Times.Once);

        _dataStoreMock.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowAcademicPeriodNotFoundException_WhenAcademicPeriodDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = CreateRequest();

        _dataStoreMock.Setup(x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync((AcademicPeriod?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(
            () => _useCase.ExecuteAsync(tenantId, academicPeriodId, request, correlationId, CancellationToken.None));

        _dataStoreMock.Verify(
            x => x.GetByIdAsync(
                academicPeriodId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _dataStoreMock.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _dataStoreMock.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDuplicateAcademicPeriodCodeException_WhenNewCodeAlreadyExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = CreateAcademicPeriod(tenantId, "2026-1", "Periodo 2026-1");

        var request = CreateRequest(code: " 2026-2 ", name: "Periodo actualizado");

        _dataStoreMock.Setup(x => x.GetByIdAsync(
                academicPeriod.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        _dataStoreMock.Setup(x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateAcademicPeriodCodeException>(
            () => _useCase.ExecuteAsync(tenantId, academicPeriod.Id, request, correlationId, CancellationToken.None));

        _dataStoreMock.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                tenantId,
                "2026-2",
                It.IsAny<CancellationToken>()),
            Times.Once);

        _dataStoreMock.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCodeDoesNotChange_ShouldNotCheckCodeExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var academicPeriod = CreateAcademicPeriod(tenantId, "2026-1", "Periodo 2026-1");

        var request = CreateRequest(code: " 2026-1 ", name: "Periodo 2026-1 actualizado");

        _dataStoreMock.Setup(x => x.GetByIdAsync(
                academicPeriod.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        _dataStoreMock.Setup(x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(tenantId, academicPeriod.Id, request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("2026-1", result.Code);
        Assert.Equal("Periodo 2026-1 actualizado", result.Name);
        Assert.Equal(correlationId, result.CorrelationId);

        _dataStoreMock.Verify(
            x => x.AcademicPeriodCodeExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _dataStoreMock.Verify(
            x => x.UpdateAcademicPeriodWithOutboxAsync(
                It.IsAny<AcademicPeriod>(),
                It.IsAny<AcademicPeriodUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static AcademicPeriod CreateAcademicPeriod(Guid tenantId, string code, string name)
    {
        return new AcademicPeriod(
            tenantId,
            code,
            name,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 5),
            new DateOnly(2026, 1, 20),
            new DateOnly(2026, 2, 15),
            new DateOnly(2026, 3, 31),
            new DateOnly(2026, 4, 30),
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 6, 30));
    }

    private static PatchAcademicPeriodRequest CreateRequest(string code = "2026-2", string name = "Periodo 2026-2")
    {
        return new PatchAcademicPeriodRequest
        {
            Code = code,
            Name = name,
            StartDate = new DateOnly(2026, 7, 1),
            EndDate = new DateOnly(2026, 12, 31),
            AcademicLoadProcessStartDate = new DateOnly(2026, 6, 1),
            AcademicLoadProcessEndDate = new DateOnly(2026, 6, 15),
            EnrollmentProcessStartDate = new DateOnly(2026, 7, 1),
            EnrollmentProcessEndDate = new DateOnly(2026, 7, 20),
            PlanningSubmissionDate = new DateOnly(2026, 8, 15),
            FirstPartialGradeReportDate = new DateOnly(2026, 9, 30),
            SecondPartialGradeReportDate = new DateOnly(2026, 10, 31),
            ThirdPartialGradeReportDate = new DateOnly(2026, 11, 30),
            FinalMinutesSubmissionDate = new DateOnly(2026, 12, 15)
        };
    }

}