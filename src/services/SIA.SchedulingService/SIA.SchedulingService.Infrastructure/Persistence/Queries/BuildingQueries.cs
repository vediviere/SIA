using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;


namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class BuildingQueries : IBuildingQueries
{
    private readonly SchedulingDbContext _dbContext;

    public BuildingQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Building?>  GetByIdAsync(Guid tenantId, Guid buildingId, CancellationToken cancellationToken)
    {
        return _dbContext.Buildings.AsNoTracking().FirstOrDefaultAsync(building =>  building.TenantId == tenantId && building.Id == buildingId, cancellationToken);
    }
}
