using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class GroupQueries : IGroupQueries
{
    private readonly SchedulingDbContext _dbContext;

    public GroupQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Group?> GetByIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        return _dbContext.Groups.FirstOrDefaultAsync(group => group.TenantId == tenantId && group.Id == groupId, cancellationToken);
    }
}