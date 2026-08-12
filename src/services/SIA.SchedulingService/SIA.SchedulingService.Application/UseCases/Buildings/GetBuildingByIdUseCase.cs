using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.DTOs.Building;
using SIA.SchedulingService.Application.Interfaces.Queries;

namespace SIA.SchedulingService.Application.UseCases.Buildings;

public sealed class GetBuildingByIdUseCase
{
    private readonly IBuildingQueries _queries;

    public GetBuildingByIdUseCase(IBuildingQueries queries)
    {
        _queries = queries;
    }

    public async Task<BuildingDto?> ExecuteAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken)
    {
        var building = await _queries.GetByIdAsync(tenantId, buildingId, cancellationToken);

        if (building == null)
        {
            throw new BuildingNotFoundException(buildingId);
        }

        return new BuildingDto
        {
            Id = building.Id,
            TenantId = building.TenantId,
            Code = building.Code,
            Name = building.Name,
            Description = building.Description,
            Status = building.Status,
            CreatedAtUtc = building.CreatedAtUtc,
            UpdatedAtUtc = building.UpdatedAtUtc
        };
    }
}
