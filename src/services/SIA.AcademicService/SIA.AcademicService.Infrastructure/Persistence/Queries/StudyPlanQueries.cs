using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries
{
    public sealed class StudyPlanQueries : IStudyPlanQueries
    {
        private readonly AcademicDbContext _dbContext;

        public StudyPlanQueries(AcademicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken)
        {
            return await _dbContext.StudyPlans
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(
                                        x => x.TenantId == tenantId &&
                                             x.Id == studyPlanId,
                                        cancellationToken);
        }

        public async Task<IReadOnlyCollection<StudyPlan>> SearchAsync(StudyPlanFilter filter, CancellationToken cancellationToken)
        {
            IQueryable<StudyPlan> query = _dbContext.StudyPlans
                                     .AsNoTracking()
                                     .Where(x => x.TenantId == filter.TenantId);

            if (filter.EducationalProgramId.HasValue)
            {
                query = query.Where(x => x.EducationalProgramId == filter.EducationalProgramId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                query = query.Where(x => x.Code.Contains(filter.Code));
            }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(x => x.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Version))
            {
                query = query.Where(x => x.Version.Contains(filter.Version));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }

            query = query.Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize);

            return await query.OrderBy(x => x.Name)
                                .ThenBy(x => x.Code)
                                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<StudyPlanSubjectDto>> GetSubjectsByStudyPlanAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken)
        {
            var result = await
            (
                from sps in _dbContext.StudyPlanSubjects
                join s in _dbContext.Subjects on sps.SubjectId equals s.Id
                where sps.TenantId == tenantId
                      && sps.StudyPlanId == studyPlanId
                      && sps.Status 
                select new StudyPlanSubjectDto
                {
                    TenantId = sps.TenantId,
                    StudyPlanId = sps.StudyPlanId,
                    SubjectId = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Semester = sps.Semester,
                    Credits = sps.Credits,
                    IsRequired = sps.IsRequired,
                    Status = sps.Status
                }
            ).ToListAsync(cancellationToken);

            return result;
        }

        public async Task<StudyPlan?> GetActiveByProgramIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
        {
            return await _dbContext.StudyPlans
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.EducationalProgramId == educationalProgramId && x.Status == true)
                .OrderByDescending(x => x.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}