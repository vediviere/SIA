using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.DTOs.SupportSchedules;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class SupportScheduleQueries : ISupportScheduleQueries
{
    private readonly SchedulingDbContext _dbContext;

    public SupportScheduleQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SupportSchedule?> GetByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken)
    {
        return await _dbContext.SupportSchedules
            .Include(x => x.ClassroomLab)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == supportScheduleId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SupportSchedule>> SearchAsync(SupportScheduleFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<SupportSchedule> query = _dbContext.SupportSchedules
            .Include(x => x.ClassroomLab)
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.SupportHourId.HasValue)
        {
            query = query.Where(x => x.SupportHourId == filter.SupportHourId.Value);
        }

        if (filter.ClassroomLabId.HasValue)
        {
            query = query.Where(x => x.ClassroomLabId == filter.ClassroomLabId.Value);
        }

        if (filter.AcademicPeriodId.HasValue)
        {
            query = query.Where(x => x.AcademicPeriodId == filter.AcademicPeriodId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Day))
        {
            query = query.Where(x => x.Day == filter.Day);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                     .Take(filter.PageSize);

        return await query.OrderBy(x => x.Day)
                          .ThenBy(x => x.StartTime)
                          .ToListAsync(cancellationToken);
    }
}
