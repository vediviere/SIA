using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.ClassSchedules;

public sealed class CreateClassScheduleUseCase
{
    private readonly IClassScheduleDataStore _dataStore;

    public CreateClassScheduleUseCase(IClassScheduleDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateClassScheduleResponse> ExecuteAsync(
        CreateClassScheduleRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classSchedule = new ClassSchedule(
            request.TenantId,
            request.OfferingId,
            request.ClassroomLabId,
            request.AcademicPeriodId,
            request.Day,
            request.StartTime,
            request.EndTime);

        var integrationEvent = new ClassScheduleCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classSchedule.CreatedAtUtc,
            TenantId = classSchedule.TenantId,
            ClassScheduleId = classSchedule.Id,
            OfferingId = classSchedule.OfferingId,
            ClassroomLabId = classSchedule.ClassroomLabId,
            AcademicPeriodId = classSchedule.AcademicPeriodId,
            Day = classSchedule.Day,
            StartTime = classSchedule.StartTime,
            EndTime = classSchedule.EndTime,
            Status = classSchedule.Status,
            Version = 1
        };

        await _dataStore.AddClassScheduleWithOutboxAsync(classSchedule, integrationEvent, cancellationToken);

        return new CreateClassScheduleResponse
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
            CreatedAtUtc = classSchedule.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}