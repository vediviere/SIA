using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Contracts.Requests.SupportSchedules;
using SIA.SchedulingService.Contracts.Responses.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.SupportSchedules;

public sealed class CreateSupportScheduleUseCase
{
    private readonly ISupportScheduleDataStore _dataStore;

    public CreateSupportScheduleUseCase(ISupportScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateSupportScheduleResponse> ExecuteAsync(
        CreateSupportScheduleRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var supportSchedule = new SupportSchedule(
            request.TenantId,
            request.SupportHourId,
            request.ClassroomLabId,
            request.AcademicPeriodId,
            request.Day,
            request.StartTime,
            request.EndTime);

        var integrationEvent = new SupportScheduleCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = supportSchedule.CreatedAtUtc,
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

        await _dataStore.AddSupportScheduleWithOutboxAsync(supportSchedule, integrationEvent, cancellationToken);

        return new CreateSupportScheduleResponse
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
            CreatedAtUtc = supportSchedule.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}