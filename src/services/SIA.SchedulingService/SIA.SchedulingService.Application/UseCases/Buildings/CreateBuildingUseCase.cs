using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.Requests.Building;
using SIA.SchedulingService.Contracts.Responses.Building;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.Buildings;

public sealed class CreateBuildingUseCase
{
    private readonly IBuildingDataStore _dataStore;

    public CreateBuildingUseCase(IBuildingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateBuildingResponse> ExecuteAsync(CreateBuildingRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExist = await _dataStore.BuildingCodeExistsAsync(request.TenantId, normalizedCode, cancellationToken);

        if (codeExist)
        {
            throw new DuplicateBuildingCodeException(normalizedCode);
        }

        var building = new Building(
            request.TenantId,
            normalizedCode,
            request.Name,
            request.Description ?? string.Empty);

        var integrationEvent = new BuildingCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = building.CreatedAtUtc,
            TenantId = building.TenantId,
            BuildingId = building.Id,
            Code = building.Code,
            Name = building.Name,
            Description = building.Description,
            Status = building.Status,
            Version = 1
        };

        await _dataStore.AddBuildingWithOutboxAsync(building, integrationEvent, cancellationToken);

        return new CreateBuildingResponse
        {
            Id = building.Id,
            TenantId = building.TenantId,
            Code = building.Code,
            Name = building.Name,
            Description = building.Description,
            Status = building.Status,
            CreatedAtUtc = building.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
