using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;


namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class RestoreClassroomLabUseCase
{
    private readonly IClassroomLabDataStore _dataStore;

    public RestoreClassroomLabUseCase(IClassroomLabDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid classroomLabId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroomLab = await _dataStore.GetClassroomLabByIdAsync(tenantId, classroomLabId, cancellationToken);

        if (classroomLab is null)
        {
            throw new ClassroomLabNotFoundException(classroomLabId);
        }

        classroomLab.Restore();

        var integrationEvent = new ClassroomLabRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomLab.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroomLab.TenantId,
            ClassroomLabId = classroomLab.Id,
            Status = classroomLab.Status,
            Version = 1
        };

        await _dataStore.RestoreClassroomLabWithOutboxAsync(classroomLab, integrationEvent, cancellationToken);
    }
}