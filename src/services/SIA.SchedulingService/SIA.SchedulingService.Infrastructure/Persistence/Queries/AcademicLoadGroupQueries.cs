using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities.Academics;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class AcademicLoadGroupQueries : IAcademicLoadGroupQueries
{
    private readonly SchedulingDbContext _dbContext;

    public AcademicLoadGroupQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AcademicLoadGroup?> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        return await _dbContext.AcademicLoadGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.GroupId == groupId, cancellationToken);
    }
    public async Task<IReadOnlyCollection<AcademicLoadGroup>> GetByAcademicLoadIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken)
    {
        return await _dbContext.AcademicLoadGroups
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.AcademicLoadId == academicLoadId)
            .ToListAsync(cancellationToken);
    }
}