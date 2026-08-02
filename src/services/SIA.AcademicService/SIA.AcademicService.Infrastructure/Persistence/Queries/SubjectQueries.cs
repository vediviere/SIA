using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.Subjects;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries;

public sealed class SubjectQueries : ISubjectQueries
{
    private readonly AcademicDbContext _dbContext;

    public SubjectQueries(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subject?> GetByIdAsync(Guid tenantId,Guid subjectId,CancellationToken cancellationToken)
    {
        return await _dbContext.Subjects
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x => x.TenantId == tenantId &&
                                     x.Id == subjectId,
                                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Subject>> SearchAsync(SubjectFilter filter,CancellationToken cancellationToken)
    {
        IQueryable<Subject> query = _dbContext.Subjects
                                    .AsNoTracking()
                                    .Where(x => x.TenantId == filter.TenantId);

        if (filter.StudyPlanId.HasValue)
        {
            query = query.Where(x => x.StudyPlanId == filter.StudyPlanId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Code))
        {
            query = query.Where(x => x.Code.Contains(filter.Code));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(x => x.Name.Contains(filter.Name));
        }

        if (filter.Semester.HasValue)
        {
            query = query.Where(x => x.Semester == filter.Semester.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

        return await query.OrderBy(x => x.Semester)
                            .ThenBy(x => x.Code)
                            .ToListAsync(cancellationToken);
    }
}