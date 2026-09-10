using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.ClassSchedules;

public sealed class UpdateClassScheduleUseCase
{
    private readonly IClassScheduleDataStore _dataStore;

    public UpdateClassScheduleUseCase(IClassScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateClassScheduleResponse> ExecuteAsync(
        Guid tenantId,
        Guid classScheduleId,
        UpdateClassScheduleRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classSchedule = await _dataStore.GetClassScheduleByIdAsync(tenantId, classScheduleId, cancellationToken);

        if (classSchedule is null)
        {
            throw new ClassScheduleNotFoundException(classScheduleId);
        }

        classSchedule.Update(
            request.Day,
            request.StartTime,
            request.EndTime);

        var integrationEvent = new ClassScheduleUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classSchedule.TenantId,
            ClassScheduleId = classSchedule.Id,
            Day = classSchedule.Day,
            StartTime = classSchedule.StartTime,
            EndTime = classSchedule.EndTime,
            Status = classSchedule.Status,
            Version = 1
        };

        await _dataStore.UpdateClassScheduleWithOutboxAsync(classSchedule, integrationEvent, cancellationToken);

        return new UpdateClassScheduleResponse
        {
            Id = classSchedule.Id,
            TenantId = classSchedule.TenantId,
            OfferingId = classSchedule.OfferingId,
            ClassroomLabId = classSchedule.ClassroomLabId,
            AcademicPeriodId = classSchedule.AcademicPeriodId,
            Day = classSchedule.Day,
            StartTime = classSchedule.StartTime,
            EndTime = classSchedule.EndTime,
            Status = classSchedule.Status,
            UpdatedAtUtc = classSchedule.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}