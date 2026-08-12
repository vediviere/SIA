using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.DTOs.Classrooms;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;


namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class ClassroomLabQueries : IClassroomLabQueries
{
    private readonly SchedulingDbContext _dbContext;

    public ClassroomLabQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassroomLab?> GetByIdAsync(Guid tenantId, Guid classroomLabId, CancellationToken cancellationToken)
    {
        return await _dbContext.ClassroomLabs 
            .Include(x => x.ClassroomType)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == classroomLabId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ClassroomLab>> SearchAsync(ClassroomLabFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<ClassroomLab> query = _dbContext.ClassroomLabs 
            .Include(x => x.ClassroomType)
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(x => x.BuildingId == filter.BuildingId.Value);
        }

        if (filter.ClassroomTypeId.HasValue)
        {
            query = query.Where(x => x.ClassroomTypeId == filter.ClassroomTypeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Code))
        {
            query = query.Where(x => x.Code.Contains(filter.Code));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(x => x.Name.Contains(filter.Name));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
                     .Take(filter.PageSize);

        return await query.OrderBy(x => x.Code).ToListAsync(cancellationToken);
    }
}