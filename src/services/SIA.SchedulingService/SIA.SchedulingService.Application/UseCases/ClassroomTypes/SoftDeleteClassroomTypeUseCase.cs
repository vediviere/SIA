using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;

namespace SIA.SchedulingService.Application.UseCases.ClassroomTypes;

public sealed class SoftDeleteClassroomTypeUseCase
{
    private readonly IClassroomTypeDataStore _dataStore;

    public SoftDeleteClassroomTypeUseCase(IClassroomTypeDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<SoftDeleteClassroomTypeResponse> ExecuteAsync(
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

        classroomType.SoftDelete();

        var integrationEvent = new ClassroomTypeDeletedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomType.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroomType.TenantId,
            ClassroomTypeId = classroomType.Id,
            Status = classroomType.Status,
            Version = 1
        };

        await _dataStore.SoftDeleteClassroomTypeWithOutboxAsync(classroomType, integrationEvent, cancellationToken);

        return new SoftDeleteClassroomTypeResponse
        {
            Id = classroomType.Id,
            Status = classroomType.Status,
            UpdatedAtUtc = classroomType.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}