using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.Responses.Classrooms;



namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class SoftDeleteClassroomUseCase
{
    private readonly IClassroomDataStore _dataStore;

    public SoftDeleteClassroomUseCase(IClassroomDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<SoftDeleteClassroomResponse> ExecuteAsync(
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

        classroom.SoftDelete();

        var integrationEvent = new ClassroomDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroom.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroom.TenantId,
            ClassroomId = classroom.Id,
            Status = classroom.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteClassroomWithOutboxAsync(classroom, integrationEvent, cancellationToken);

        return new SoftDeleteClassroomResponse
        {
            Id = classroom.Id,
            Status = classroom.Status,
            UpdatedAtUtc = classroom.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}