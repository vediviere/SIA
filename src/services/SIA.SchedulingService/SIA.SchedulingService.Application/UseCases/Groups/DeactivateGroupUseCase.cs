using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.Groups;

public sealed class DeactivateGroupUseCase
{
    private readonly IGroupDataStore _dataStore;

    public DeactivateGroupUseCase(IGroupDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task ExecuteAsync(Guid tenantId, Guid id, Guid correlationId, CancellationToken cancellation)
    {
        var group = await _dataStore.GetByIdAsync(tenantId, id, cancellation);

        if (group == null)
        {
            throw new GroupNotFoundException(id);
        }

        group.Deactivate();

        var integrationEvent = new GroupDeactivatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = group.UpdatedAtUtc!.Value,
            TenantId = group.TenantId,
            GroupId = group.Id,
            EducationalProgramId = group.EducationalProgramId,
            Status = group.Status,
            Version = 1,
        };

        await _dataStore.DeactivateGroupWithOutboxAsync(group, integrationEvent, cancellation);
    }
}