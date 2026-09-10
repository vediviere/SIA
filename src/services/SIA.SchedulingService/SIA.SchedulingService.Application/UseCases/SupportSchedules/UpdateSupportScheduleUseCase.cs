using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.Requests.SupportSchedules;
using SIA.SchedulingService.Contracts.Responses.SupportSchedules;
using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;

using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportSchedules;

public sealed class UpdateSupportScheduleUseCase
{
    private readonly ISupportScheduleDataStore _dataStore;

    public UpdateSupportScheduleUseCase(ISupportScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateSupportScheduleResponse> ExecuteAsync(
        Guid tenantId,
        Guid supportScheduleId,
        UpdateSupportScheduleRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportSchedule = await _dataStore.GetSupportScheduleByIdAsync(tenantId, supportScheduleId, cancellationToken);

        if (supportSchedule is null)
        {
            throw new SupportScheduleNotFoundException(supportScheduleId);
        }

        supportSchedule.Update(
            request.Day,
            request.StartTime,
            request.EndTime);

        var integrationEvent = new SupportScheduleUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportSchedule.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = supportSchedule.TenantId,
            SupportScheduleId = supportSchedule.Id,
            SupportHourId = supportSchedule.SupportHourId,
            ClassroomLabId = supportSchedule.ClassroomLabId,
            AcademicPeriodId = supportSchedule.AcademicPeriodId,
            Day = supportSchedule.Day,
            StartTime = supportSchedule.StartTime,
            EndTime = supportSchedule.EndTime,
            Status = supportSchedule.Status,
            Version = 1
        };

        await _dataStore.UpdateSupportScheduleWithOutboxAsync(supportSchedule, integrationEvent, cancellationToken);

        return new UpdateSupportScheduleResponse
        {
            Id = supportSchedule.Id,
            TenantId = supportSchedule.TenantId,
            SupportHourId = supportSchedule.SupportHourId,
            ClassroomLabId = supportSchedule.ClassroomLabId,
            AcademicPeriodId = supportSchedule.AcademicPeriodId,
            Day = supportSchedule.Day,
            StartTime = supportSchedule.StartTime,
            EndTime = supportSchedule.EndTime,
            Status = supportSchedule.Status,
            UpdatedAtUtc = supportSchedule.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}