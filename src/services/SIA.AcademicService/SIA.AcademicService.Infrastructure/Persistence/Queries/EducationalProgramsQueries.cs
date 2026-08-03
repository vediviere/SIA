using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SIA.AcademicService.Infrastructure.Persistence.Queries;

public sealed class EducationalProgramsQueries : IEducationalProgramsQueries
{
    private readonly AcademicDbContext _dbContext;

    public EducationalProgramsQueries(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EducationalProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.EducationalPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<EducationalProgram>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.EducationalPrograms
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}