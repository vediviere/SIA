using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries;

public sealed class StudyPlanQueries : IStudyPlanQueries
{
    private readonly AcademicDbContext _dbContext;

    public StudyPlanQueries(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.StudyPlans.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<StudyPlan>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.StudyPlans.AsNoTracking().ToListAsync(cancellationToken);
    }
}