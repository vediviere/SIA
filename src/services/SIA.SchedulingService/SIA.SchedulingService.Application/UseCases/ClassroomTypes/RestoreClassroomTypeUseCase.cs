using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;

namespace SIA.SchedulingService.Application.UseCases.ClassroomTypes;

public sealed class RestoreClassroomTypeUseCase
{
    private readonly IClassroomTypeDataStore _dataStore;

    public RestoreClassroomTypeUseCase(IClassroomTypeDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Guid classroomTypeId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroomType = await _dataStore.GetClassroomTypeByIdAsync(tenantId, classroomTypeId, cancellationToken);

        if (classroomType is null)
        {
            throw new ClassroomTypeNotFoundException(classroomTypeId);
        }

        classroomType.Restore();

        var integrationEvent = new ClassroomTypeRestoredIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomType.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroomType.TenantId,
            ClassroomTypeId = classroomType.Id,
            Status = classroomType.Status,
            Version = 1
        };

        await _dataStore.RestoreClassroomTypeWithOutboxAsync(classroomType, integrationEvent, cancellationToken);
    }
}