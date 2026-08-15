using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class ActivateTeachingSupportHoursUseCase
{
    private readonly ITeachingSupportHoursDataStore _dataStore;

    public ActivateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellationToken)
    {
        var teachingSupportHours = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);
        if (teachingSupportHours is null)
        {
            throw new TeachingSupportHoursNotFoundException(id);
        }
        teachingSupportHours.Activate();
        var integrationEvent = new TeachingSupportHoursActivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = teachingSupportHours.UpdatedAtUtc!.Value,
            TenantId = teachingSupportHours.TenantId,
            SupportHourId = teachingSupportHours.Id,
            Status = teachingSupportHours.Status,
            Version = 1
        };

        await _dataStore.ActivateTeachingSupportHoursWithOutboxAsync(teachingSupportHours, integrationEvent, cancellationToken);
    }
}