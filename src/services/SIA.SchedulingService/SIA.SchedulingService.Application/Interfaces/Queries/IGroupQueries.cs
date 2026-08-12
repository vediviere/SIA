using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IGroupQueries
{
    Task<Group?> GetByIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
}
