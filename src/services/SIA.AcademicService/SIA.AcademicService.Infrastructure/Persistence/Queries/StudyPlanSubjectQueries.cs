using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.StudyPlanSubjects;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries;

public sealed class StudyPlanSubjectQueries : IStudyPlanSubjectQueries
{
    private readonly AcademicDbContext _dbContext;

    public StudyPlanSubjectQueries(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudyPlanSubject?> GetByIdAsync(Guid tenantId, Guid studyPlanSubjectId, CancellationToken cancellationToken)
    {
        return await _dbContext.StudyPlanSubjects
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x => x.TenantId == tenantId &&
                                     x.Id == studyPlanSubjectId,
                                cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyPlanSubject>> SearchAsync(StudyPlanSubjectFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<StudyPlanSubject> query = _dbContext.StudyPlanSubjects
                                    .AsNoTracking()
                                    .Where(x => x.TenantId == filter.TenantId);

        if (filter.StudyPlanId.HasValue)
        {
            query = query.Where(x => x.StudyPlanId == filter.StudyPlanId.Value);
        }

        if (filter.SubjectId.HasValue)
        {
            query = query.Where(x => x.SubjectId == filter.SubjectId.Value);
        }

        if (filter.IsRequired.HasValue)
        {
            query = query.Where(x => x.IsRequired == filter.IsRequired.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

        return await query.OrderBy(x => x.StudyPlanId)
                            .ThenBy(x => x.Semester)
                            .ToListAsync(cancellationToken);
    }
}