using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.DTOs.ServiceComplementaries;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;


namespace SIA.AcademicService.Infrastructure.Persistence.Queries;

public sealed class ServiceComplementaryQueries : IServiceComplementaryQueries
{
    private readonly AcademicDbContext _dbContext;

    public ServiceComplementaryQueries(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceComplementary?> GetByIdAsync(Guid tenantId, Guid serviceComplementaryId, CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceComplementaries
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x => x.TenantId == tenantId &&
                                     x.Id == serviceComplementaryId,
                                cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServiceComplementary>> SearchAsync(ServiceComplementaryFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<ServiceComplementary> query = _dbContext.ServiceComplementaries
                                    .AsNoTracking()
                                    .Where(x => x.TenantId == filter.TenantId);

        if (filter.StudyPlanId.HasValue)
        {
            query = query.Where(x => x.StudyPlanId == filter.StudyPlanId.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(x => x.Type == filter.Type.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

        return await query.OrderBy(x => x.StudyPlanId)
                            .ThenBy(x => x.Type)
                            .ToListAsync(cancellationToken);
    }
}