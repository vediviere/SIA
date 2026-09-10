using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.Responses.SupportSchedules;


namespace SIA.SchedulingService.Application.UseCases.SupportSchedules;

public sealed class SoftDeleteSupportScheduleUseCase
{
    private readonly ISupportScheduleDataStore _dataStore;

    public SoftDeleteSupportScheduleUseCase(ISupportScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid supportScheduleId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportSchedule = await _dataStore.GetSupportScheduleByIdAsync(tenantId, supportScheduleId, cancellationToken);

        if (supportSchedule is null)
        {
            throw new SupportScheduleNotFoundException(supportScheduleId);
        }

        supportSchedule.SoftDelete();

        var integrationEvent = new SupportScheduleDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = supportSchedule.TenantId,
            SupportScheduleId = supportSchedule.Id,
            Status = supportSchedule.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteSupportScheduleWithOutboxAsync(supportSchedule, integrationEvent, cancellationToken);
    }
}