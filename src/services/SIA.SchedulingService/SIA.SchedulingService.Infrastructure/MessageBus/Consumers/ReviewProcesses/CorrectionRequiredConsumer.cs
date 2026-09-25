using MassTransit;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;

namespace SIA.SchedulingService.Infrastructure.MessageBus.Consumers.ReviewProcesses;

public sealed class CorrectionRequiredConsumer : IConsumer<CorrectionRequiredEvent>
{
    private const string SourceService = "SIA.WorkflowService";
    private readonly ApplyCorrectionUseCase _useCase;

    public CorrectionRequiredConsumer(ApplyCorrectionUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task Consume(ConsumeContext<CorrectionRequiredEvent> context)
    {
        var integrationEvent = context.Message;

        await _useCase.ExecuteAsync(
          integrationEvent.TenantId,
          integrationEvent.ProposalId,
          integrationEvent.Version,
          integrationEvent.EventId, 
          ReviewEventTypes.ProposalRequiresCorrectionV1,
          SourceService,
          integrationEvent.CorrelationId,
          context.CancellationToken);
    }
}