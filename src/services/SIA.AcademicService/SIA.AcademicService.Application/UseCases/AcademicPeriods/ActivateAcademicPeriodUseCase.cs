
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class ActivateAcademicPeriodUseCase
{
  private readonly IAcademicPeriodsDataStore _dataStore;

  public ActivateAcademicPeriodUseCase(IAcademicPeriodsDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<ActivateAcademicPeriodResponse> ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicPeriod = await _dataStore.GetByIdAsync(id, cancellationToken);

    if (academicPeriod is null)
    {
      throw new AcademicPeriodNotFoundException(id);
    }

    academicPeriod.Activate();

    var integrationEvent = new AcademicPeriodActivatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicPeriod.UpdatedAtUtc!.Value,
      TenantId = academicPeriod.TenantId,
      AcademicPeriodId = academicPeriod.Id,
      Status = academicPeriod.Status,
      Version = 1
    };

    await _dataStore.ActivateAcademicPeriodWithOutboxAsync(academicPeriod, integrationEvent, cancellationToken);

    return new ActivateAcademicPeriodResponse
    {
      Id = academicPeriod.Id,
      Status = academicPeriod.Status,
      UpdatedAtUtc = academicPeriod.UpdatedAtUtc,
      CorrelationId = correlationId
    };

  }

}
