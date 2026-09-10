using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class CreateTeachingSupportHoursUseCase
{
  private readonly ITeachingSupportHoursDataStore _dataStore;
  private readonly IAcademicLoadDataStore _academicLoadDataStore;
  private readonly AcademicLoadSupportHoursCalculator _supportHoursCalculator;
  private readonly ProposalValidator _proposalValidator;

  public CreateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore, IAcademicLoadDataStore academicLoadDataStore, AcademicLoadSupportHoursCalculator supportHoursCalculator, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _academicLoadDataStore = academicLoadDataStore;
    _supportHoursCalculator = supportHoursCalculator;
    _proposalValidator = proposalValidator;
  }

  public async Task<CreateTeachingSupportHoursResponse> ExecuteAsync(CreateTeachingSupportHoursRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicLoad = await _academicLoadDataStore.GetByIdAsync(request.TenantId, request.AcademicLoadId, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(request.AcademicLoadId);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    var exists = await _dataStore.ExistsByActivityAndAcademicLoadAsync(request.TenantId, request.ActivityId, request.AcademicLoadId, cancellationToken);

    if (exists)
    {
      throw new DuplicateTeachingSupportHoursException(request.ActivityId, request.AcademicLoadId);
    }

    var teachingSupportHours = new TeachingSupportHour(request.TenantId, request.ActivityId, request.AcademicLoadId, request.Hours);

    await _supportHoursCalculator.RecalculateAsync(academicLoad, teachingSupportHours, cancellationToken);

    var integrationEvent = new TeachingSupportHoursCreatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = teachingSupportHours.CreatedAtUtc,
      TenantId = teachingSupportHours.TenantId,
      SupportHourId = teachingSupportHours.Id,
      ActivityId = teachingSupportHours.ActivityId,
      AcademicLoadId = teachingSupportHours.AcademicLoadId,
      Hours = teachingSupportHours.Hours,
      Status = teachingSupportHours.Status,
      Version = 1
    };

    await _dataStore.AddTeachingSupportHoursWithOutboxAsync(teachingSupportHours, academicLoad, integrationEvent, cancellationToken);

    return new CreateTeachingSupportHoursResponse
    {
      Id = teachingSupportHours.Id,
      TenantId = teachingSupportHours.TenantId,
      ActivityId = teachingSupportHours.ActivityId,
      AcademicLoadId = teachingSupportHours.AcademicLoadId,
      Hours = teachingSupportHours.Hours,
      Status = teachingSupportHours.Status,
      CreatedAtUtc = teachingSupportHours.CreatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
