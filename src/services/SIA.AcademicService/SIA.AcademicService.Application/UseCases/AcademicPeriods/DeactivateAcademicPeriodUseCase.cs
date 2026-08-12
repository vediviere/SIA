
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class DeactivateAcademicPeriodUseCase
{
  private readonly IAcademicPeriodsDataStore _dataStore;

  public DeactivateAcademicPeriodUseCase(IAcademicPeriodsDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<DeactivateAcademicPeriodResponse> ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicPeriod = await _dataStore.GetByIdAsync(id, cancellationToken);

    if (academicPeriod is null)
    {
      throw new AcademicPeriodNotFoundException(id);
    }

    academicPeriod.Deactivate();

    var integrationEvent = new AcademicPeriodDeactivatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicPeriod.UpdatedAtUtc!.Value,
      TenantId = academicPeriod.TenantId,
      AcademicPeriodId = academicPeriod.Id,
      Status = academicPeriod.Status,
      Version = 1
    };

    await _dataStore.DeactivateAcademicPeriodWithOutboxAsync(academicPeriod, integrationEvent, cancellationToken);

    return new DeactivateAcademicPeriodResponse
    {
      Id = academicPeriod.Id,
      Status = academicPeriod.Status,
      UpdatedAtUtc = academicPeriod.UpdatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
