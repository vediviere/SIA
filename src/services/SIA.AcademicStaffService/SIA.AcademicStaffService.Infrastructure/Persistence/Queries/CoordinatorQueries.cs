using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.DTOs.Coordinators;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Queries;

public sealed class CoordinatorQueries : ICoordinatorQueries
{
    private readonly AcademicStaffDbContext _dbContext;

    public CoordinatorQueries(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Coordinator?> GetByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken)
    {
        return await _dbContext.Coordinators
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == coordinatorId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Coordinator>> SearchAsync(CoordinatorFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Coordinator> query = _dbContext.Coordinators
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

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

        return await query.OrderBy(x => x.PersonId)
            .ToListAsync(cancellationToken);
    }
}