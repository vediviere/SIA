using SIA.WorkflowService.Application.Common.Exceptions.ReviewProcesses;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed class ApproveUseCase
{
  private readonly IReviewStore _store;

  public ApproveUseCase(IReviewStore store)
  {
    _store = store;
  }

  public async Task ExecuteAsync(ApproveCommand command, CancellationToken cancellationToken)
  {
    var process = await _store.GetByIdAsync(command.TenantId, command.ProcessId, cancellationToken);

    if (process is null)
    {
      throw new ReviewNotFoundException(command.ProcessId);
    }

    if (process.Status != ReviewStatus.InReview)
    {
      throw new AlreadyDecidedException(process.Id);
    }

    var decidedAtUtc = DateTime.UtcNow;

    process.Approve(command.DecidedBy, decidedAtUtc, command.CorrelationId);

    var integrationEvent = new ApprovedEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = command.CorrelationId,
      OccurredAtUtc = decidedAtUtc,
      TenantId = process.TenantId,
      ReviewProcessId = process.Id,
      ProposalId = process.AcademicLoadProposalId,
      DecidedBy = command.DecidedBy,
      Version = process.Version
    };

    await _store.SaveDecisionAsync(process, integrationEvent, ReviewEventTypes.ProposalApprovedV1, command.CorrelationId, cancellationToken);
  }
}
