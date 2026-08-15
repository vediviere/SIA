
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class UpdateTeachingSupportHoursUseCase
{
    private readonly ITeachingSupportHoursDataStore _dataStore;

    public UpdateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore)
    {
        _dataStore = dataStore;
    }
    public async Task<UpdateTeachingSupportHoursResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateTeachingSupportHoursRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var teachingSupportHours = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (teachingSupportHours is null)
        {
            throw new TeachingSupportHoursNotFoundException(id);
        }
        teachingSupportHours.Update(request.Hours);

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

        await _dataStore.UpdateTeachingSupportHoursWithOutboxAsync(teachingSupportHours, integrationEvent, cancellationToken);

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