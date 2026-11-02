using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadGroups;
using SIA.SchedulingService.Domain.Entities.Academics;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;

public sealed class AssignTutorUseCase
{
    private readonly IAcademicLoadGroupDataStore _dataStore;
    private readonly IGroupDataStore _groupDataStore;
    private readonly IAcademicLoadDataStore _academicLoadDataStore;

    public AssignTutorUseCase(
        IAcademicLoadGroupDataStore dataStore,
        IGroupDataStore groupDataStore,
        IAcademicLoadDataStore academicLoadDataStore)
    {
        _dataStore = dataStore;
        _groupDataStore = groupDataStore;
        _academicLoadDataStore = academicLoadDataStore;
    }

    public async Task<AssignTutorResponse> ExecuteAsync(Guid tenantId, AssignTutorRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var group = await _groupDataStore.GetByIdAsync(tenantId, request.GroupId, cancellationToken);
        if (group is null || !group.Status)
        {
            throw new GroupNotEligibleForTutorException(request.GroupId);
        }
        var academicLoad = await _academicLoadDataStore.GetByIdAsync(tenantId, request.AcademicLoadId, cancellationToken);
        if (academicLoad is null || !academicLoad.Status)
        {
            throw new AcademicLoadNotFoundException(request.AcademicLoadId);
        }
        var exists = await _dataStore.ExistsByGroupIdAsync(tenantId, request.GroupId, cancellationToken);
        if (exists)
        {
            throw new GroupAlreadyHasTutorException(request.GroupId);
        }
        var academicLoadGroup = new AcademicLoadGroup(
            tenantId,
            request.AcademicLoadId,
            request.GroupId
        );
        var integrationEvent = new TutorAssignedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = DateTime.UtcNow,
            TenantId = academicLoadGroup.TenantId,
            AcademicLoadGroupId = academicLoadGroup.Id,
            GroupId = academicLoadGroup.GroupId,
            AcademicLoadId = academicLoadGroup.AcademicLoadId,
            Hours = academicLoadGroup.Hours,
            Version = 1
        };
        await _dataStore.AddAcademicLoadGroupWithOutboxAsync(academicLoadGroup, integrationEvent, cancellationToken);
        return new AssignTutorResponse
        {
            Id = academicLoadGroup.Id,
            TenantId = academicLoadGroup.TenantId,
            GroupId = academicLoadGroup.GroupId,
            AcademicLoadId = academicLoadGroup.AcademicLoadId,
            Hours = academicLoadGroup.Hours,
            CorrelationId = correlationId
        };
    }
}