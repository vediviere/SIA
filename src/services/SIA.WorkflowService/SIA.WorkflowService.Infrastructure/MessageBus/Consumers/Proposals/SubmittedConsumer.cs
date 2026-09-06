using MassTransit;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;

namespace SIA.WorkflowService.Infrastructure.MessageBus.Consumers.Proposals;

public sealed class SubmittedConsumer : IConsumer<ProposalSubmittedForReviewIntegrationEvent>
{
  private const string SourceService = "SIA.SchedulingService";
  private readonly CreateUseCase _useCase;

  public SubmittedConsumer(CreateUseCase useCase)
  {
    _useCase = useCase;
  }

  public async Task Consume(ConsumeContext<ProposalSubmittedForReviewIntegrationEvent> context)
  {
    var integrationEvent = context.Message;

    var command = new CreateCommand
    {
      EventId = integrationEvent.EventId,
      EventType = SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1,
      SourceService = SourceService,
      TenantId = integrationEvent.TenantId,
      AcademicLoadProposalId = integrationEvent.ProposalId,
      DivisionHeadId = integrationEvent.DivisionHeadId,
      Version = integrationEvent.Version,
      SubmittedAtUtc = integrationEvent.OccurredAtUtc,
      CorrelationId = integrationEvent.CorrelationId
    };

    await _useCase.ExecuteAsync(command, context.CancellationToken);
  }
}
