using SIA.AcademicStaffService.Application.Common.Exceptions;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Contracts.Requests.Coordinators;
using SIA.AcademicStaffService.Contracts.Responses.Coordinators;

namespace SIA.AcademicStaffService.Application.UseCases.Coordinators;

public sealed class UpdateCoordinatorUseCase
{
    private readonly ICoordinatorDataStore _dataStore;

    public UpdateCoordinatorUseCase(ICoordinatorDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateCoordinatorResponse> ExecuteAsync(
        Guid tenantId,
        Guid coordinatorId,
        UpdateCoordinatorRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var coordinator = await _dataStore.GetCoordinatorByIdAsync(tenantId, coordinatorId, cancellationToken);

        if (coordinator is null)
        {
            throw new CoordinatorNotFoundException(coordinatorId);
        }

        coordinator.Update(request.AcademicDegree);

        var integrationEvent = new CoordinatorUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = coordinator.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = coordinator.TenantId,
            CoordinatorId = coordinator.Id,
            PersonId = coordinator.PersonId,
            AcademicDegree = coordinator.AcademicDegree,
            Status = coordinator.Status,
            Version = 1
        };

        await _dataStore.UpdateCoordinatorWithOutboxAsync(coordinator, integrationEvent, cancellationToken);

        return new UpdateCoordinatorResponse
        {
            Id = coordinator.Id,
            TenantId = coordinator.TenantId,
            PersonId = coordinator.PersonId,
            AcademicDegree = coordinator.AcademicDegree,
            Status = coordinator.Status,
            UpdatedAtUtc = coordinator.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}