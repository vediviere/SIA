using SIA.SchedulingService.Domain.Entities.Academics;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IAcademicLoadGroupQueries
{
    Task<AcademicLoadGroup?> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AcademicLoadGroup>> GetByAcademicLoadIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken);
}