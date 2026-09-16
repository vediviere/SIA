using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

namespace SIA.WorkflowService.Tests.Application.UseCases.ReviewProcesses;

public sealed class ReturnUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithObservation_ShouldReturnAndCreateOutbox()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var process = CreateProcess();
    await dbContext.ReviewProcesses.AddAsync(process);
    await dbContext.SaveChangesAsync();

    var useCase = new ReturnUseCase(new ReviewStore(dbContext));
    var command = new ReturnCommand
    {
      ProcessId = process.Id,
      TenantId = process.TenantId,
      DecidedBy = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid(),
      Observations =
      [
        new ObservationInput
        {
          TargetType = ObservationTarget.Offering,
          TargetId = Guid.NewGuid(),
          Description = "Revisar la asignación de horas."
        }
      ]
    };

    await useCase.ExecuteAsync(command, CancellationToken.None);

    var savedProcess = await dbContext.ReviewProcesses.AsNoTracking().SingleAsync();
    var observation = await dbContext.ReviewObservations.AsNoTracking().SingleAsync();
    var outbox = await dbContext.OutboxMessages.AsNoTracking().SingleAsync();
    var integrationEvent = JsonSerializer.Deserialize<CorrectionRequiredEvent>(outbox.Payload);

    Assert.Equal(ReviewStatus.RequiresCorrection, savedProcess.Status);
    Assert.Equal(command.DecidedBy, savedProcess.DecidedBy);
    Assert.Equal(command.CorrelationId, savedProcess.DecisionCorrelationId);
    Assert.Equal(process.Id, observation.ReviewProcessId);
    Assert.Equal(process.TenantId, observation.TenantId);
    Assert.Equal(command.DecidedBy, observation.CreatedBy);
    Assert.Equal(command.CorrelationId, observation.CorrelationId);
    Assert.Equal(ReviewEventTypes.ProposalRequiresCorrectionV1, outbox.EventType);
    Assert.NotNull(integrationEvent);
    Assert.Equal(process.Id, integrationEvent.ReviewProcessId);
    Assert.Equal(process.AcademicLoadProposalId, integrationEvent.ProposalId);
  }

  [Fact]
  public async Task ExecuteAsync_WithoutObservations_ShouldThrowArgumentException()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlite(connection).Options;
    await using var dbContext = new WorkflowDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var process = CreateProcess();
    await dbContext.ReviewProcesses.AddAsync(process);
    await dbContext.SaveChangesAsync();

    var useCase = new ReturnUseCase(new ReviewStore(dbContext));
    var command = new ReturnCommand
    {
      ProcessId = process.Id,
      TenantId = process.TenantId,
      DecidedBy = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid(),
      Observations = []
    };

    await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(command, CancellationToken.None));

    Assert.Equal(ReviewStatus.InReview, process.Status);
    Assert.Empty(await dbContext.ReviewObservations.ToListAsync());
    Assert.Empty(await dbContext.OutboxMessages.ToListAsync());
  }

  private static ReviewProcess CreateProcess()
  {
    return new ReviewProcess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow, Guid.NewGuid());
  }
}
