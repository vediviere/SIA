using MassTransit;
using MassTransit.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Infrastructure.MessageBus.Consumers.Proposals;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

namespace SIA.WorkflowService.Tests.Infrastructure.Consumers;

public sealed class SubmittedConsumerTests
{
  [Fact]
  public async Task Consume_WithSubmittedProposal_ShouldCreateReviewProcess()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var services = new ServiceCollection();

    services.AddLogging();
    services.AddDbContext<WorkflowDbContext>(options => options.UseSqlite(connection));
    services.AddScoped<IReviewStore, ReviewStore>();
    services.AddScoped<CreateUseCase>();

    services.AddMassTransitTestHarness(configurator =>
    {
      configurator.AddConsumer<SubmittedConsumer>();
    });

    await using var provider = services.BuildServiceProvider(true);

    using (var scope = provider.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
      await dbContext.Database.EnsureCreatedAsync();
    }

    var harness = await provider.StartTestHarness();

    var integrationEvent = new ProposalSubmittedForReviewIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid(),
      OccurredAtUtc = DateTime.UtcNow,
      TenantId = Guid.NewGuid(),
      ProposalId = Guid.NewGuid(),
      EducationalProgramId = Guid.NewGuid(),
      AcademicPeriodId = Guid.NewGuid(),
      DivisionHeadId = Guid.NewGuid(),
      Status = true,
      Version = 1
    };

    await harness.Bus.Publish(integrationEvent);

    var consumerHarness = harness.GetConsumerHarness<SubmittedConsumer>();

    Assert.True(await harness.Consumed.Any<ProposalSubmittedForReviewIntegrationEvent>());
    Assert.True(await consumerHarness.Consumed.Any<ProposalSubmittedForReviewIntegrationEvent>());

    using var verificationScope = provider.CreateScope();
    var verificationContext = verificationScope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

    var process = Assert.Single(await verificationContext.ReviewProcesses.AsNoTracking().ToListAsync());
    var inbox = Assert.Single(await verificationContext.InboxMessages.AsNoTracking().ToListAsync());

    Assert.Equal(integrationEvent.TenantId, process.TenantId);
    Assert.Equal(integrationEvent.ProposalId, process.AcademicLoadProposalId);
    Assert.Equal(integrationEvent.DivisionHeadId, process.DivisionHeadId);
    Assert.Equal(integrationEvent.Version, process.Version);
    Assert.Equal(integrationEvent.EventId, inbox.Id);
    Assert.NotNull(inbox.ProcessedAtUtc);
  }
}
