using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.Requests.Group;
using SIA.SchedulingService.Contracts.Responses.Group;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.UseCases.Groups;

public sealed class CreateGroupUseCase
{
    private readonly IGroupDataStore _dataStore;

    public CreateGroupUseCase(IGroupDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateGroupResponse> ExecuteAsync(CreateGroupRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var normalizedName = request.GroupName.Trim().ToUpperInvariant();
        var normalidedShift = request.Shift.Trim().ToUpperInvariant();

        var groupExists = await _dataStore.GroupExistsAsync(request.TenantId, request.EducationalProgramId, request.Shift, request.GroupName, cancellationToken);

        if (groupExists)
        {
            throw new DuplicateGroupException(normalizedName, normalidedShift);
        }

        var group = new Group(
            request.TenantId,
            request.EducationalProgramId,
            normalizedName,
            normalidedShift,
            request.Capacity);

        var integrationEvent = new GroupCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = group.CreatedAtUtc,
            TenantId = group.TenantId,
            GroupId = group.Id,
            EducationalProgramId = group.EducationalProgramId,
            GroupName = group.GroupName,
            Shift = group.Shift,
            Capacity = group.Capacity,
            Status = group.Status,
            Version = 1
        };

        await _dataStore.AddGroupWithOutboxAsync(group, integrationEvent, cancellationToken);

        return new CreateGroupResponse
        {
            Id = group.Id,
            TenantId = group.TenantId,
            EducationalProgramId = group.EducationalProgramId,
            GroupName = group.GroupName,
            Shift = group.Shift,
            Capacity = group.Capacity,
            Status = group.Status,
            CreatedAtUtc = group.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
