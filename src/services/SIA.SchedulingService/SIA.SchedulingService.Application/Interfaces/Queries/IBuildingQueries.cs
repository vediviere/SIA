using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IBuildingQueries
{
    Task<Building?> GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken);
}