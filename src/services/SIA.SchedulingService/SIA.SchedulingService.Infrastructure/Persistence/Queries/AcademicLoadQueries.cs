using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class AcademicLoadQueries : IAcademicLoadQueries
{
    private readonly SchedulingDbContext _dbContext;

    public AcademicLoadQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicLoad.FirstOrDefaultAsync(academicLoad => academicLoad.TenantId == tenantId && academicLoad.Id == academicLoadId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AcademicLoad>> GetActiveByTeacherAndPeriodAsync(Guid tenantId, Guid teacherId, Guid academicPeriodId, CancellationToken cancellationToken)
    {
        return await _dbContext.AcademicLoad.AsNoTracking().Where(x => x.TenantId == tenantId && x.TeacherId == teacherId && x.AcademicPeriodId == academicPeriodId && x.Status).ToListAsync(cancellationToken);
    }
}