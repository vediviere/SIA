using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Domain.Entities;

namespace SIA.WorkflowService.Application.UseCases.ReviewProcesses;

public sealed class CreateUseCase
{
  private readonly IReviewStore _store;

  public CreateUseCase(IReviewStore store)
  {
    _store = store;
  }

  public async Task<bool> ExecuteAsync(CreateCommand command, CancellationToken cancellationToken)
  {
    if (await _store.WasProcessedAsync(command.EventId, cancellationToken))
    {
      return false;
    }

    var process = new ReviewProcess(command.TenantId, command.AcademicLoadProposalId, command.DivisionHeadId, command.Version, command.SubmittedAtUtc, command.CorrelationId);

    await _store.CreateAsync(process, command.EventId, command.EventType, command.SourceService, cancellationToken);

    return true;
  }
}
