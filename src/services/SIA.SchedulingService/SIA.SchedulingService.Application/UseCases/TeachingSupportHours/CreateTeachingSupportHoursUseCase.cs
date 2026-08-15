using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Responses.TeachingSupportHours;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class CreateTeachingSupportHoursUseCase
{
    private readonly ITeachingSupportHoursDataStore _dataStore;

    public CreateTeachingSupportHoursUseCase(ITeachingSupportHoursDataStore dataStore)
    {
        _dataStore = dataStore;
    }
    public async Task<CreateTeachingSupportHoursResponse> ExecuteAsync(CreateTeachingSupportHoursRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var existsTSH = await _dataStore.ExistsByActivityAndAcademicLoadAsync(request.ActivityId, request.AcademicLoadId, cancellationToken);

        if (existsTSH)
        {
            throw new DuplicateTeachingSupportHoursException(request.ActivityId, request.AcademicLoadId);
        }

        var teachingSupportHours = new TeachingSupportHour(
            request.TenantId,
            request.ActivityId,
            request.AcademicLoadId,
            request.Hours);

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

        await _dataStore.AddTeachingSupportHoursWithOutboxAsync(teachingSupportHours, integrationEvent, cancellationToken);

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
