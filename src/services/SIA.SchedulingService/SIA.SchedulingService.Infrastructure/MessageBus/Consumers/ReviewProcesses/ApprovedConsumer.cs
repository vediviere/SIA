using MassTransit;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;

namespace SIA.SchedulingService.Infrastructure.MessageBus.Consumers.ReviewProcesses;

public sealed class ApprovedConsumer : IConsumer<ApprovedEvent>
{
    private const string SourceService = "SIA.WorkflowService";
    private readonly ApplyApprovedUseCase _useCase;

    public ApprovedConsumer(ApplyApprovedUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task Consume(ConsumeContext<ApprovedEvent> context)
    {
        var integrationEvent = context.Message;

        await _useCase.ExecuteAsync(
          integrationEvent.TenantId,
          integrationEvent.ProposalId,
          integrationEvent.EventId,
          ReviewEventTypes.ProposalApprovedV1,
          SourceService,
          integrationEvent.CorrelationId,
          context.CancellationToken);
    }
}