using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.DTOs.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Application.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class SupportActivityQueries : ISupportActivityQueries
{
    private readonly SchedulingDbContext _dbContext;

    public SupportActivityQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SupportActivity?> GetByIdAsync(Guid tenantId, Guid supportActivityId, CancellationToken cancellationToken)
    {
        return await _dbContext.SupportActivities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == supportActivityId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SupportActivity>> SearchAsync(SupportActivityFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<SupportActivity> query = _dbContext.SupportActivities
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (!string.IsNullOrWhiteSpace(filter.Activity))
        {
            query = query.Where(x => x.Activity.Contains(filter.Activity));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                     .Take(filter.PageSize);

        return await query.OrderBy(x => x.Activity)
                          .ToListAsync(cancellationToken);
    }
}