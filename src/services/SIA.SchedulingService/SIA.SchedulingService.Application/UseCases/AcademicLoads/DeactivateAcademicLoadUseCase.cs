using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoads;

public sealed class DeactivateAcademicLoadUseCase
{
  private readonly IAcademicLoadDataStore _dataStore;
  private readonly ProposalValidator _proposalValidator;

  public DeactivateAcademicLoadUseCase(IAcademicLoadDataStore dataStore, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _proposalValidator = proposalValidator;
  }

  public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicLoad = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(id);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    academicLoad.Deactivate();

    var integrationEvent = new AcademicLoadDeactivatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicLoad.UpdatedAtUtc!.Value,
      TenantId = academicLoad.TenantId,
      ProposalId = academicLoad.ProposalId,
      AcademicLoadId = academicLoad.Id,
      Status = academicLoad.Status,
      Version = 1
    };

    await _dataStore.DeactivateAcademicLoadWithOutboxAsync(academicLoad, integrationEvent, cancellationToken);
  }
}
