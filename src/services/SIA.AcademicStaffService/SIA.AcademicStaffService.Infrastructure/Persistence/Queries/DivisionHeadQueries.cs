using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.DTOs.DivisionManagers;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Queries;

public sealed class DivisionHeadQueries : IDivisionHeadQueries
{
    private readonly AcademicStaffDbContext _dbContext;

    public DivisionHeadQueries(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DivisionHead?> GetByIdAsync(Guid tenantId, Guid divisionManagerId, CancellationToken cancellationToken)
    {
        return await _dbContext.DivisionHeads
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == divisionManagerId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<DivisionHead>> SearchAsync(DivisionHeadFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<DivisionHead> query = _dbContext.DivisionHeads
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.ProgramId.HasValue)
        {
            query = query.Where(x => x.ProgramId == filter.ProgramId.Value);
        }

        if (filter.PersonId.HasValue)
        {
            query = query.Where(x => x.PersonId == filter.PersonId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.OrderBy(x => x.ProgramId)
            .ToListAsync(cancellationToken);
    }
}