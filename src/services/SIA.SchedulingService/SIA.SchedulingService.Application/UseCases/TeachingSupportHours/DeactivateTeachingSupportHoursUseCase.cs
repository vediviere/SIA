using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class DeactivateTeachingSupportHoursUseCase
{
  private readonly ITeachingSupportHoursDataStore _dataStore;
  private readonly IAcademicLoadDataStore _academicLoadDataStore;
  private readonly AcademicLoadSupportHoursCalculator _supportHoursCalculator;
  private readonly ProposalValidator _proposalValidator;

  public DeactivateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore, IAcademicLoadDataStore academicLoadDataStore, AcademicLoadSupportHoursCalculator supportHoursCalculator, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _academicLoadDataStore = academicLoadDataStore;
    _supportHoursCalculator = supportHoursCalculator;
    _proposalValidator = proposalValidator;
  }

  public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
  {
    var teachingSupportHours = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);
    if (teachingSupportHours is null)
    {
      throw new TeachingSupportHoursNotFoundException(id);
    }

    var academicLoad = await _academicLoadDataStore.GetByIdAsync(tenantId, teachingSupportHours.AcademicLoadId, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(teachingSupportHours.AcademicLoadId);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    teachingSupportHours.Deactivate();
    await _supportHoursCalculator.RecalculateAsync(academicLoad, teachingSupportHours, cancellationToken);

    var integrationEvent = new TeachingSupportHoursDeactivatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = teachingSupportHours.UpdatedAtUtc!.Value,
      TenantId = teachingSupportHours.TenantId,
      SupportHourId = teachingSupportHours.Id,
      Status = teachingSupportHours.Status,
      Version = 1
    };

    await _dataStore.DeactivateTeachingSupportHoursWithOutboxAsync(teachingSupportHours, academicLoad, integrationEvent, cancellationToken);
  }
}
