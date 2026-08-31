
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class UpdateTeachingSupportHoursUseCase
{
  private readonly ITeachingSupportHoursDataStore _dataStore;
  private readonly IAcademicLoadDataStore _academicLoadDataStore;
  private readonly AcademicLoadSupportHoursCalculator _supportHoursCalculator;
  private readonly ProposalValidator _proposalValidator;

  public UpdateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore, IAcademicLoadDataStore academicLoadDataStore, AcademicLoadSupportHoursCalculator supportHoursCalculator, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _academicLoadDataStore = academicLoadDataStore;
    _supportHoursCalculator = supportHoursCalculator;
    _proposalValidator = proposalValidator;
  }

  public async Task<UpdateTeachingSupportHoursResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateTeachingSupportHoursRequest request, Guid correlationId, CancellationToken cancellationToken)
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

    teachingSupportHours.Update(request.Hours);
    await _supportHoursCalculator.RecalculateAsync(academicLoad, teachingSupportHours, cancellationToken);

    var integrationEvent = new TeachingSupportHoursUpdatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = teachingSupportHours.UpdatedAtUtc!.Value,
      TenantId = teachingSupportHours.TenantId,
      SupportHourId = teachingSupportHours.Id,
      Hours = teachingSupportHours.Hours,
      Status = teachingSupportHours.Status,
      Version = 1
    };

    await _dataStore.UpdateTeachingSupportHoursWithOutboxAsync(teachingSupportHours, academicLoad, integrationEvent, cancellationToken);

    return new UpdateTeachingSupportHoursResponse
    {
      Id = teachingSupportHours.Id,
      TenantId = teachingSupportHours.TenantId,
      ActivityId = teachingSupportHours.ActivityId,
      AcademicLoadId = teachingSupportHours.AcademicLoadId,
      Hours = teachingSupportHours.Hours,
      Status = teachingSupportHours.Status,
      CreatedAtUtc = teachingSupportHours.CreatedAtUtc,
      UpdatedAtUtc = teachingSupportHours.UpdatedAtUtc,
      CorrelationId = correlationId
    };

  }

}
