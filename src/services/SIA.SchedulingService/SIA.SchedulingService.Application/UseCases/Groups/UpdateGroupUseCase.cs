
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Contracts.Responses.Group;

namespace SIA.SchedulingService.Application.UseCases.Groups;

public sealed class UpdateGroupUseCase
{
    private readonly IGroupDataStore _dataStore;

    public UpdateGroupUseCase(IGroupDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateGroupResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateGroupRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var group = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if(group == null)
        {
            throw new GroupNotFoundException(id);
        }

        var normalizedName = request.GroupName.Trim().ToUpperInvariant();
        var normalizedShift = request.Shift.Trim().ToUpperInvariant();

        if (normalizedName != group.GroupName || normalizedShift != group.Shift)
        {
            var groupExists = await _dataStore.GroupExistsAsync(tenantId, group.EducationalProgramId, normalizedShift, normalizedName, cancellationToken);

            if (groupExists)
            {
                throw new DuplicateGroupException(normalizedName, normalizedShift);
            }
        }

        group.Update(normalizedName, normalizedShift, request.Capacity);

        var integrationEvent = new GroupUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = group.UpdatedAtUtc!.Value,
            TenantId = group.TenantId,
            GroupId = group.Id,
            EducationalProgramId = group.EducationalProgramId,
            GroupName = group.GroupName,
            Shift = group.Shift,
            Capacity = group.Capacity,
            Status = group.Status,
            Version = 1
        };

        await _dataStore.UpdateGroupWithOutboxAsync(group, integrationEvent, cancellationToken);

        return new UpdateGroupResponse
        {
            Id = group.Id,
            TenantId = group.TenantId,
            EducationalProgramId = group.EducationalProgramId,
            GroupId = group.GroupName,
            Shift = group.Shift,
            Capacity = group.Capacity,
            Status = group.Status,
            CreatedAtUtc = group.CreatedAtUtc,
            UpdatedAtUtc = group.UpdatedAtUtc,
            CorrelationId = correlationId
        };

    }
}
