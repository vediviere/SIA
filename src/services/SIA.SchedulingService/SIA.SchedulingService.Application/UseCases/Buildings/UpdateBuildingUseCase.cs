using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Contracts.Responses.Building;

namespace SIA.SchedulingService.Application.UseCases.Buildings;

public sealed class UpdateBuildingUseCase 
{
    private readonly IBuildingDataStore _dataStore;

    public UpdateBuildingUseCase(IBuildingDataStore dataStores)
    {
        _dataStore = dataStores;
    }

    public async Task<UpdateBuildingResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateBuildingRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var building = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (building == null)
        {
            throw new BuildingNotFoundException(id);
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (normalizedCode != building.Code)
        {
            var codeExists = await _dataStore.BuildingCodeExistsAsync(tenantId, normalizedCode, cancellationToken);

            if (codeExists)
            {
                throw new DuplicateBuildingCodeException(normalizedCode);
            }
        }

        building.Update(normalizedCode, request.Name, request.Description ?? string.Empty);

        var integrationEvent = new BuildingUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = building.UpdatedAtUtc!.Value,
            TenantId = building.TenantId,
            BuildingId = building.Id,
            Code = building.Code,
            Name = building.Name,
            Description = building.Description,
            Status = building.Status,
            Version = 1
        };

        await _dataStore.UpdateBuildingWithOutboxAsync(building, integrationEvent, cancellationToken);

        return new UpdateBuildingResponse
        {
            Id = building.Id,
            TenantId = building.TenantId,
            Code = building.Code,
            Name = building.Name,
            Description = building.Description,
            Status = building.Status,
            CreatedAtUtc = building.CreatedAtUtc,
            UpdatedAtUtc = building.UpdatedAtUtc,
            CorrelationId = correlationId
        };

    }
}