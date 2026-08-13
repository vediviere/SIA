using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.DTOs.ClassSchedules;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class ClassScheduleQueries : IClassScheduleQueries
{
    private readonly SchedulingDbContext _dbContext;

    public ClassScheduleQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassSchedule?> GetByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClassSchedules
            .Include(x => x.ClassroomLab)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == classScheduleId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ClassSchedule>> SearchAsync(ClassScheduleFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<ClassSchedule> query = _dbContext.ClassSchedules
            .Include(x => x.ClassroomLab)
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.OfferingId.HasValue)
        {
            query = query.Where(x => x.OfferingId == filter.OfferingId.Value);
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