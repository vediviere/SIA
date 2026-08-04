using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.EducationalProgram;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries
{
    public sealed class EducationalProgramQueries : IEducationalProgramQueries
    {
        private readonly AcademicDbContext _dbContext;

        public EducationalProgramQueries(AcademicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EducationalProgram?> GetByIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
        {
            return await _dbContext.EducationalPrograms
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(
                                        x => x.TenantId == tenantId &&
                                             x.Id == educationalProgramId,
                                        cancellationToken);
        }

        public async Task<IReadOnlyCollection<EducationalProgram>> SearchAsync(EducationalProgramFilter filter, CancellationToken cancellationToken)
        {
            IQueryable<EducationalProgram> query = _dbContext.EducationalPrograms
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

            if (!string.IsNullOrWhiteSpace(filter.Level))
            {
                query = query.Where(x => x.Level.Contains(filter.Level));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(
                                x => x.Status == filter.Status.Value);
            }

            query = query.Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize);

            return await query.OrderBy(x => x.Name)
                                .ThenBy(x => x.Code)
                                .ToListAsync(cancellationToken);
        }
    }
}
