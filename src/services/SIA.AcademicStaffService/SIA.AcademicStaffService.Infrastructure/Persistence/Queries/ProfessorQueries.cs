using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.DTOs.Professors;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Queries;

public sealed class ProfessorQueries : IProfessorQueries
{
    private readonly AcademicStaffDbContext _dbContext;

    public ProfessorQueries(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Professor?> GetByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken)
    {
        return await _dbContext.Professors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == professorId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Professor>> SearchAsync(ProfessorFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Professor> query = _dbContext.Professors
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.PersonId.HasValue)
        {
            query = query.Where(x => x.PersonId == filter.PersonId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.ContractType))
        {
            query = query.Where(x => x.ContractType.Contains(filter.ContractType));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.OrderBy(x => x.ContractType)
            .ThenBy(x => x.AcademicDegree)
            .ToListAsync(cancellationToken);
    }
}