using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

namespace SIA.WorkflowService.Tests.Application.UseCases.ReviewProcesses;

public sealed class CreateUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidCommand_ShouldCreateProcessAndInbox()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var useCase = new CreateUseCase(new ReviewStore(dbContext));
    var command = BuildCommand();

    var result = await useCase.ExecuteAsync(command, CancellationToken.None);

    var process = Assert.Single(await dbContext.ReviewProcesses.ToListAsync());
    var inbox = Assert.Single(await dbContext.InboxMessages.ToListAsync());

    Assert.True(result);
    Assert.Equal(command.TenantId, process.TenantId);
    Assert.Equal(command.AcademicLoadProposalId, process.AcademicLoadProposalId);
    Assert.Equal(command.DivisionHeadId, process.DivisionHeadId);
    Assert.Equal(command.Version, process.Version);
    Assert.Equal(ReviewStatus.InReview, process.Status);
    Assert.Equal(command.EventId, inbox.Id);
    Assert.NotNull(inbox.ProcessedAtUtc);
  }

  [Fact]
  public async Task ExecuteAsync_WithProcessedEvent_ShouldNotCreateDuplicate()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var useCase = new CreateUseCase(new ReviewStore(dbContext));
    var command = BuildCommand();

    var firstResult = await useCase.ExecuteAsync(command, CancellationToken.None);
    var secondResult = await useCase.ExecuteAsync(command, CancellationToken.None);

    Assert.True(firstResult);
    Assert.False(secondResult);
    Assert.Equal(1, await dbContext.ReviewProcesses.CountAsync());
    Assert.Equal(1, await dbContext.InboxMessages.CountAsync());
  }

  private static CreateCommand BuildCommand()
  {
    return new CreateCommand
    {
      EventId = Guid.NewGuid(),
      EventType = "ProposalSubmittedForReviewIntegrationEvent.v1",
      SourceService = "SIA.SchedulingService",
      TenantId = Guid.NewGuid(),
      AcademicLoadProposalId = Guid.NewGuid(),
      DivisionHeadId = Guid.NewGuid(),
      Version = 1,
      SubmittedAtUtc = DateTime.UtcNow,
      CorrelationId = Guid.NewGuid()
    };
  }
}
