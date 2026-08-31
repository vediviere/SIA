using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries
{
    public sealed class AcademicPeriodQueries : IAcademicPeriodQueries
    {
        private readonly AcademicDbContext _dbContext;

        public AcademicPeriodQueries(AcademicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AcademicPeriod?> GetByIdAsync(Guid tenantId, Guid academicPeriodId, CancellationToken cancellationToken)
        {
            return await _dbContext.AcademicPeriods
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(
                                        x => x.TenantId == tenantId
                                             && x.Id == academicPeriodId,
                                        cancellationToken);
        }

        public async Task<IReadOnlyCollection<AcademicPeriod>> SearchAsync(AcademicPeriodFilter filter, CancellationToken cancellationToken)
        {
            IQueryable<AcademicPeriod> query = _dbContext.AcademicPeriods
                                                    .AsNoTracking()
                                                    .Where(x => x.TenantId == filter.TenantId);

            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                query = query.Where(x => x.Code.Contains(filter.Code));
            }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(x => x.Name.Contains(filter.Name));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }

            query = query.Skip((filter.Page - 1) * filter.PageSize)
                   .Take(filter.PageSize);

            return await query.OrderByDescending(x => x.StartDate)
                                .ToListAsync(cancellationToken);
        }

        public async Task<AcademicPeriod?> GetActivePeriodAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            return await _dbContext.AcademicPeriods
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.Status == true)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}