using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.Common.Exceptions.ReviewProcesses;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

namespace SIA.WorkflowService.Tests.Application.UseCases.ReviewProcesses;

public sealed class ApproveUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidCommand_ShouldApproveAndCreateOutbox()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var process = CreateProcess();
    await dbContext.ReviewProcesses.AddAsync(process);
    await dbContext.SaveChangesAsync();

    var useCase = new ApproveUseCase(new ReviewStore(dbContext));
    var command = new ApproveCommand
    {
      ProcessId = process.Id,
      TenantId = process.TenantId,
      DecidedBy = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid()
    };

    await useCase.ExecuteAsync(command, CancellationToken.None);

    var savedProcess = await dbContext.ReviewProcesses.AsNoTracking().SingleAsync();
    var outbox = await dbContext.OutboxMessages.AsNoTracking().SingleAsync();
    var integrationEvent = JsonSerializer.Deserialize<ApprovedEvent>(outbox.Payload);

    Assert.Equal(ReviewStatus.Approved, savedProcess.Status);
    Assert.Equal(command.DecidedBy, savedProcess.DecidedBy);
    Assert.NotNull(savedProcess.DecidedAtUtc);
    Assert.Equal(command.CorrelationId, savedProcess.DecisionCorrelationId);
    Assert.Equal(ReviewEventTypes.ProposalApprovedV1, outbox.EventType);
    Assert.Equal(command.CorrelationId, outbox.CorrelationId);
    Assert.NotNull(integrationEvent);
    Assert.Equal(process.Id, integrationEvent.ReviewProcessId);
    Assert.Equal(process.AcademicLoadProposalId, integrationEvent.ProposalId);
    Assert.Equal(command.DecidedBy, integrationEvent.DecidedBy);
  }

  [Fact]
  public async Task ExecuteAsync_WhenAlreadyDecided_ShouldThrowConflict()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var process = CreateProcess();
    await dbContext.ReviewProcesses.AddAsync(process);
    await dbContext.SaveChangesAsync();

    var useCase = new ApproveUseCase(new ReviewStore(dbContext));
    var command = new ApproveCommand
    {
      ProcessId = process.Id,
      TenantId = process.TenantId,
      DecidedBy = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid()
    };

    await useCase.ExecuteAsync(command, CancellationToken.None);

    await Assert.ThrowsAsync<AlreadyDecidedException>(() => useCase.ExecuteAsync(command with
    {
      CorrelationId = Guid.NewGuid()
    }, CancellationToken.None));

    Assert.Equal(1, await dbContext.OutboxMessages.CountAsync());
  }

  [Fact]
  public async Task ExecuteAsync_WithAnotherTenant_ShouldThrowNotFound()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var process = CreateProcess();
    await dbContext.ReviewProcesses.AddAsync(process);
    await dbContext.SaveChangesAsync();

    var useCase = new ApproveUseCase(new ReviewStore(dbContext));
    var command = new ApproveCommand
    {
      ProcessId = process.Id,
      TenantId = Guid.NewGuid(),
      DecidedBy = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid()
    };

    await Assert.ThrowsAsync<ReviewNotFoundException>(() => useCase.ExecuteAsync(command, CancellationToken.None));

    Assert.Equal(ReviewStatus.InReview, process.Status);
    Assert.Empty(await dbContext.OutboxMessages.ToListAsync());
  }

  private static ReviewProcess CreateProcess()
  {
    return new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());
  }
}
