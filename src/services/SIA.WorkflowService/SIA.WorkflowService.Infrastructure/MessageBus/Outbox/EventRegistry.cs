using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;

namespace SIA.WorkflowService.Infrastructure.MessageBus.Outbox;

public static class EventRegistry
{
  public static OutboxEventRegistry Create()
  {
    return new OutboxEventRegistry()
      .Register<ApprovedEvent>(ReviewEventTypes.ProposalApprovedV1)
      .Register<CorrectionRequiredEvent>(ReviewEventTypes.ProposalRequiresCorrectionV1);
  }
}
