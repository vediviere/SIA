
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;

namespace SIA.SchedulingService.Application.UseCases.Groups;

public sealed class ActivateGroupUseCase
{
    private readonly IGroupDataStore _dataStore;

    public ActivateGroupUseCase(IGroupDataStore dataStore)
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

        group.Activate();

        var integrationEvent = new GroupActivateIntegrationEvent
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

        await _dataStore.ActivateGroupWithOutboxAsync(group, integrationEvent, cancellation);
    }
}

