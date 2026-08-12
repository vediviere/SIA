using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.DTOs.ClassroomTypes;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;


namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class ClassroomTypeQueries : IClassroomTypeQueries
{
    private readonly SchedulingDbContext _dbContext;

    public ClassroomTypeQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassroomType?> GetByIdAsync(Guid tenantId, Guid classroomTypeId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClassroomTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == classroomTypeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ClassroomType>> SearchAsync(ClassroomTypeFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<ClassroomType> query = _dbContext.ClassroomTypes
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(x => x.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Code))
        {
            query = query.Where(x => x.Code.Contains(filter.Code));
        }

        if (!string.IsNullOrWhiteSpace(filter.Description))
        {
            query = query.Where(x => x.Description.Contains(filter.Description));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                     .Take(filter.PageSize);

        return await query.OrderBy(x => x.Name)
                          .ToListAsync(cancellationToken);
    }
}