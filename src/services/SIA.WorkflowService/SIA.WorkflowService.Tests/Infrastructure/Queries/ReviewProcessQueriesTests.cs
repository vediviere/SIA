using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.Queries;

namespace SIA.WorkflowService.Tests.Infrastructure.Queries;

public sealed class ReviewProcessQueriesTests
{
    [Fact]
    public async Task GetInReviewAsync_WhenProcessesExist_ShouldReturnOnlyInReviewProcessesForTenant()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var tenantId = Guid.NewGuid();

        var inReviewProcess = CreateProcess(tenantId);
        var approvedProcess = CreateProcess(tenantId);

        approvedProcess.Approve(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        await dbContext.ReviewProcesses.AddRangeAsync(
            inReviewProcess,
            approvedProcess);

        await dbContext.SaveChangesAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetInReviewAsync(
            tenantId,
            CancellationToken.None);

        // Assert
        var returnedProcess = Assert.Single(result);

        Assert.Equal(inReviewProcess.Id, returnedProcess.Id);
        Assert.Equal(
            inReviewProcess.AcademicLoadProposalId,
            returnedProcess.AcademicLoadProposalId);
        Assert.Equal(
            inReviewProcess.DivisionHeadId,
            returnedProcess.DivisionHeadId);
        Assert.Equal(inReviewProcess.Version, returnedProcess.Version);
        Assert.Equal(
            inReviewProcess.SubmittedAtUtc,
            returnedProcess.SubmittedAtUtc);
    }

    [Fact]
    public async Task GetInReviewAsync_WhenAnotherTenantHasProcesses_ShouldNotReturnThem()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();

        var tenantAProcess = CreateProcess(tenantAId);
        var tenantBProcess = CreateProcess(tenantBId);

        await dbContext.ReviewProcesses.AddRangeAsync(
            tenantAProcess,
            tenantBProcess);

        await dbContext.SaveChangesAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetInReviewAsync(
            tenantAId,
            CancellationToken.None);

        // Assert
        var returnedProcess = Assert.Single(result);

        Assert.Equal(tenantAProcess.Id, returnedProcess.Id);
        Assert.DoesNotContain(
            result,
            process => process.Id == tenantBProcess.Id);
    }

    [Fact]
    public async Task GetInReviewAsync_WhenNoProcessesExist_ShouldReturnEmptyList()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetInReviewAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProcessBelongsToTenant_ShouldReturnProcess()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var tenantId = Guid.NewGuid();
        var process = CreateProcess(tenantId);

        await dbContext.ReviewProcesses.AddAsync(process);
        await dbContext.SaveChangesAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetByIdAsync(
            tenantId,
            process.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(process.Id, result.Id);
        Assert.Equal(
            process.AcademicLoadProposalId,
            result.AcademicLoadProposalId);
        Assert.Equal(
            process.DivisionHeadId,
            result.DivisionHeadId);
        Assert.Equal(process.Version, result.Version);
        Assert.Equal((int)process.Status, result.Status);
        Assert.Equal(process.SubmittedAtUtc, result.SubmittedAtUtc);
        Assert.Equal(process.CorrelationId, result.CorrelationId);
        Assert.Empty(result.Observations);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProcessBelongsToAnotherTenant_ShouldReturnNull()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();

        var tenantBProcess = CreateProcess(tenantBId);

        await dbContext.ReviewProcesses.AddAsync(tenantBProcess);
        await dbContext.SaveChangesAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetByIdAsync(
            tenantAId,
            tenantBProcess.Id,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProcessDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetByIdAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProcessHasObservations_ShouldReturnObservations()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WorkflowDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var tenantId = Guid.NewGuid();
        var process = CreateProcess(tenantId);

        var decidedBy = Guid.NewGuid();
        var decidedAtUtc = DateTime.UtcNow;
        var correlationId = Guid.NewGuid();
        var targetId = Guid.NewGuid();

        var observation = new ReviewObservation(
            process.Id,
            tenantId,
            ObservationTarget.Offering,
            targetId,
            "Revisar las horas asignadas.",
            decidedBy,
            decidedAtUtc,
            correlationId);

        process.ReturnForCorrection(
            decidedBy,
            decidedAtUtc,
            correlationId,
            [observation]);

        await dbContext.ReviewProcesses.AddAsync(process);
        await dbContext.SaveChangesAsync();

        var queries = new ReviewProcessQueries(dbContext);

        // Act
        var result = await queries.GetByIdAsync(
            tenantId,
            process.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)ReviewStatus.RequiresCorrection, result.Status);

        var returnedObservation = Assert.Single(result.Observations);

        Assert.Equal(observation.Id, returnedObservation.Id);
        Assert.Equal(
            (SIA.WorkflowService.Contracts.Enums.ObservationTarget)observation.TargetType,
            returnedObservation.TargetType);
        Assert.Equal(targetId, returnedObservation.TargetId);
        Assert.Equal("Revisar las horas asignadas.", returnedObservation.Description);
        Assert.Equal(decidedBy, returnedObservation.CreatedBy);
        Assert.Equal(decidedAtUtc, returnedObservation.CreatedAtUtc);
    }

    private static ReviewProcess CreateProcess(Guid tenantId)
    {
        return new ReviewProcess(
            tenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());
    }
}