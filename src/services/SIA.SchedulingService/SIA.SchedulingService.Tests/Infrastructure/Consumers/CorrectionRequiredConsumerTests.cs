using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;
using SIA.SchedulingService.Infrastructure.MessageBus.Consumers.ReviewProcesses;
using SIA.SchedulingService.Tests.Common.Fakes;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;

namespace SIA.SchedulingService.Tests.Infrastructure.Consumers;

public sealed class CorrectionRequiredConsumerTests
{
  [Fact]
  public async Task Consume_WithCorrectionEvent_ShouldUpdateProposal()
  {
    var tenantId = Guid.NewGuid();
    var proposal = new Proposal(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    proposal.SubmitForReview();

    var dataStore = new FakeProposalDataStore(proposal);
    var services = new ServiceCollection();

    services.AddLogging();
    services.AddSingleton<IProposalDataStore>(dataStore);
    services.AddScoped<ApplyCorrectionUseCase>();

    services.AddMassTransitTestHarness(configurator =>
    {
      configurator.AddConsumer<CorrectionRequiredConsumer>();
    });

    await using var provider = services.BuildServiceProvider(true);
    var harness = await provider.StartTestHarness();

    var integrationEvent = new CorrectionRequiredEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = Guid.NewGuid(),
      OccurredAtUtc = DateTime.UtcNow,
      TenantId = tenantId,
      ReviewProcessId = Guid.NewGuid(),
      ProposalId = proposal.Id,
      DecidedBy = Guid.NewGuid(),
      Version = 1
    };

    await harness.Bus.Publish(integrationEvent);

    var consumerHarness = harness.GetConsumerHarness<CorrectionRequiredConsumer>();

    Assert.True(await harness.Consumed.Any<CorrectionRequiredEvent>());
    Assert.True(await consumerHarness.Consumed.Any<CorrectionRequiredEvent>());
    Assert.Equal(ProposalStatus.RequiresCorrection, proposal.ProposalStatus);
    Assert.Same(proposal, dataStore.AppliedDecisionProposal);
    Assert.Equal(integrationEvent.EventId, dataStore.AppliedEventId);
    Assert.Equal(ReviewEventTypes.ProposalRequiresCorrectionV1, dataStore.AppliedEventType);
    Assert.Equal("SIA.WorkflowService", dataStore.AppliedSourceService);
    Assert.Equal(integrationEvent.CorrelationId, dataStore.AppliedCorrelationId);
    Assert.Equal(1, dataStore.AppliedDecisionCount);
  }

  
}
