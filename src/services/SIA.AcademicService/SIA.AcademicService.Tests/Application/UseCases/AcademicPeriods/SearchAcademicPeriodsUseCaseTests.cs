using Moq;
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicPeriods;

public class SearchAcademicPeriodsUseCaseTests
{
    private readonly Mock<IAcademicPeriodQueries> _queriesMock;
    private readonly SearchAcademicPeriodsUseCase _useCase;

    public SearchAcademicPeriodsUseCaseTests()
    {
        _queriesMock = new Mock<IAcademicPeriodQueries>();
        _useCase = new SearchAcademicPeriodsUseCase(_queriesMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAcademicPeriodsExist_ShouldReturnAcademicPeriodDtos()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var filter = new AcademicPeriodFilter
        {
            TenantId = tenantId,
            Code = "2026-1",
            Name = "Periodo 2026-1",
            Status = true,
            Page = 1,
            PageSize = 10
        };

        var academicPeriod1 = CreateAcademicPeriod(
            tenantId,
            "2026-1",
            "Periodo 2026-1");

        var academicPeriod2 = CreateAcademicPeriod(tenantId, "2026-2", "Periodo 2026-2");

        var academicPeriods = new List<AcademicPeriod>
        {
            academicPeriod1,
            academicPeriod2
        };

        _queriesMock.Setup(x => x.SearchAsync(
                filter,
                It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriods);

        // Act
        var result = await _useCase.ExecuteAsync(filter, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var firstResult = result.First();
        var secondResult = result.Last();

        Assert.Equal(academicPeriod1.Id, firstResult.Id);
        Assert.Equal(academicPeriod1.TenantId, firstResult.TenantId);
        Assert.Equal(academicPeriod1.Code, firstResult.Code);
        Assert.Equal(academicPeriod1.Name, firstResult.Name);
        Assert.Equal(academicPeriod1.StartDate, firstResult.StartDate);
        Assert.Equal(academicPeriod1.EndDate, firstResult.EndDate);
        Assert.Equal(academicPeriod1.AcademicLoadProcessStartDate, firstResult.AcademicLoadProcessStartDate);
        Assert.Equal(academicPeriod1.AcademicLoadProcessEndDate, firstResult.AcademicLoadProcessEndDate);
        Assert.Equal(academicPeriod1.EnrollmentProcessStartDate, firstResult.EnrollmentProcessStartDate);
        Assert.Equal(academicPeriod1.EnrollmentProcessEndDate, firstResult.EnrollmentProcessEndDate);
        Assert.Equal(academicPeriod1.PlanningSubmissionDate, firstResult.PlanningSubmissionDate);
        Assert.Equal(academicPeriod1.FirstPartialGradeReportDate, firstResult.FirstPartialGradeReportDate);
        Assert.Equal(academicPeriod1.SecondPartialGradeReportDate, firstResult.SecondPartialGradeReportDate);
        Assert.Equal(academicPeriod1.ThirdPartialGradeReportDate, firstResult.ThirdPartialGradeReportDate);
        Assert.Equal(academicPeriod1.FinalMinutesSubmissionDate, firstResult.FinalMinutesSubmissionDate);
        Assert.Equal(academicPeriod1.Status, firstResult.Status);
        Assert.Equal(academicPeriod1.CreatedAtUtc, firstResult.CreatedAtUtc);
        Assert.Equal(academicPeriod1.UpdatedAtUtc, firstResult.UpdatedAtUtc);
        Assert.Equal(academicPeriod2.Id, secondResult.Id);
        Assert.Equal(academicPeriod2.Code, secondResult.Code);
        Assert.Equal(academicPeriod2.Name, secondResult.Name);

        _queriesMock.Verify(
            x => x.SearchAsync(
                filter,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoAcademicPeriodsExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        var filter = new AcademicPeriodFilter
        {
            TenantId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10
        };

        _queriesMock.Setup(x => x.SearchAsync(
                filter,
                It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<AcademicPeriod>());

        // Act
        var result = await _useCase.ExecuteAsync(filter, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _queriesMock.Verify(
            x => x.SearchAsync(
                filter,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassCancellationTokenToQuery()
    {
        // Arrange
        var filter = new AcademicPeriodFilter
        {
            TenantId = Guid.NewGuid()
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _queriesMock.Setup(x => x.SearchAsync(
                filter,
                cancellationToken)).ReturnsAsync(Array.Empty<AcademicPeriod>());

        // Act
        await _useCase.ExecuteAsync(filter, cancellationToken);

        // Assert
        _queriesMock.Verify(
            x => x.SearchAsync(
                filter,
                cancellationToken),
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
}