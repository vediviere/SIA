using SIA.WorkflowService.Application.Common.Exceptions.ReviewProcesses;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;

namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed class ReturnUseCase
{
  private readonly IReviewStore _store;

  public ReturnUseCase(IReviewStore store)
  {
    _store = store;
  }

  public async Task ExecuteAsync(ReturnCommand command, CancellationToken cancellationToken)
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

    if (command.Observations is null || command.Observations.Count == 0)
    {
      throw new ArgumentException("Debe existir al menos una observación para regresar la propuesta.", nameof(command.Observations));
    }

    var decidedAtUtc = DateTime.UtcNow;

    var observations = command.Observations.Select(input => new ReviewObservation(
      process.Id,
      process.TenantId,
      input.TargetType,
      input.TargetId,
      input.Description,
      command.DecidedBy,
      decidedAtUtc,
      command.CorrelationId)).ToArray();

    process.ReturnForCorrection(command.DecidedBy, decidedAtUtc, command.CorrelationId, observations);

    var integrationEvent = new CorrectionRequiredEvent
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

    await _store.SaveDecisionAsync(process, integrationEvent, ReviewEventTypes.ProposalRequiresCorrectionV1, command.CorrelationId, cancellationToken);
  }
}
