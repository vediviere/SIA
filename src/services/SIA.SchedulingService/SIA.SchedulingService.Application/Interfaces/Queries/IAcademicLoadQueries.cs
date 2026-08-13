using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IAcademicLoadQueries
{
    Task<AcademicLoad?> GetByIdAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken);
}