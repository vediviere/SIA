using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class TeachingSupportHoursQueries : ITeachingSupportHoursQueries
{
    private readonly SchedulingDbContext _dbContext;

    public TeachingSupportHoursQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Entities.TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.TeachingSupportHours.AsNoTracking().FirstOrDefaultAsync(hours => hours.TenantId == tenantId && hours.Id == id, cancellationToken);
    }
}