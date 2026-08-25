using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class GetAcademicPeriodByIdUseCaseTests
{
    private readonly Mock<IAcademicPeriodQueries> _queriesMock;
    private readonly GetAcademicPeriodByIdUseCase _useCase;

    public GetAcademicPeriodByIdUseCaseTests()
    {
        _queriesMock = new Mock<IAcademicPeriodQueries>();
        _useCase = new GetAcademicPeriodByIdUseCase(_queriesMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAcademicPeriodExists_ShouldReturnAcademicPeriodDto()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();

        var academicPeriod = new AcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo Académico 2026-1",
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
            new DateOnly(2026, 6, 30)
        );

        // Necesitamos que la entidad tenga el Id que vamos a consultar.
        // Si la  entidad genera el Id internamente y no permite asignarlo, usamos academicPeriod.Id.
        academicPeriodId = academicPeriod.Id;

        _queriesMock.Setup(x => x.GetByIdAsync(
                tenantId,
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

        // Act
        var result = await _useCase.ExecuteAsync(tenantId,academicPeriodId,CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(academicPeriod.Id, result.Id);
        Assert.Equal(academicPeriod.TenantId, result.TenantId);
        Assert.Equal(academicPeriod.Code, result.Code);
        Assert.Equal(academicPeriod.Name, result.Name);
        Assert.Equal(academicPeriod.StartDate, result.StartDate);
        Assert.Equal(academicPeriod.EndDate, result.EndDate);
        Assert.Equal(academicPeriod.AcademicLoadProcessStartDate,result.AcademicLoadProcessStartDate);
        Assert.Equal(academicPeriod.AcademicLoadProcessEndDate,result.AcademicLoadProcessEndDate);
        Assert.Equal(academicPeriod.EnrollmentProcessStartDate,result.EnrollmentProcessStartDate);
        Assert.Equal(academicPeriod.EnrollmentProcessEndDate,result.EnrollmentProcessEndDate);
        Assert.Equal(academicPeriod.PlanningSubmissionDate,result.PlanningSubmissionDate);
        Assert.Equal(academicPeriod.FirstPartialGradeReportDate,result.FirstPartialGradeReportDate);
        Assert.Equal(academicPeriod.SecondPartialGradeReportDate,result.SecondPartialGradeReportDate);
        Assert.Equal(academicPeriod.ThirdPartialGradeReportDate,result.ThirdPartialGradeReportDate);
        Assert.Equal(academicPeriod.FinalMinutesSubmissionDate,result.FinalMinutesSubmissionDate);
        Assert.Equal(academicPeriod.Status, result.Status);
        Assert.Equal(academicPeriod.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(academicPeriod.UpdatedAtUtc, result.UpdatedAtUtc);

        _queriesMock.Verify(
            x => x.GetByIdAsync(
                tenantId,
                academicPeriodId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAcademicPeriodDoesNotExist_ShouldThrowAcademicPeriodNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var academicPeriodId = Guid.NewGuid();

        _queriesMock.Setup(x => x.GetByIdAsync(
                tenantId,
                academicPeriodId,
                It.IsAny<CancellationToken>())).ReturnsAsync((AcademicPeriod?)null);

        // Act
        var exception = await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(
            () => _useCase.ExecuteAsync(tenantId, academicPeriodId, CancellationToken.None)
        );

        // Assert
        Assert.NotNull(exception);

        _queriesMock.Verify(
            x => x.GetByIdAsync(
                tenantId,
                academicPeriodId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}