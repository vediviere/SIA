using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.Responses.Classrooms;


namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class RestoreClassroomUseCase
{
    private readonly IClassroomDataStore _dataStore;

    public RestoreClassroomUseCase(IClassroomDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<RestoreClassroomResponse> ExecuteAsync(
        Guid tenantId,
        Guid classroomId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroom = await _dataStore.GetClassroomByIdAsync(tenantId, classroomId, cancellationToken);

        if (classroom is null)
        {
            throw new ClassroomNotFoundException(classroomId);
        }

        classroom.Restore();

        var integrationEvent = new ClassroomRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroom.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroom.TenantId,
            ClassroomId = classroom.Id,
            Status = classroom.Status,
            Version = 1
        };

        await _dataStore.RestoreClassroomWithOutboxAsync(classroom, integrationEvent, cancellationToken);

        return new RestoreClassroomResponse
        {
            Id = classroom.Id,
            Status = classroom.Status,
            UpdatedAtUtc = classroom.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
