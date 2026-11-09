using SIA.AcademicStaffService.Application.DTOs.Coordinators;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface ICoordinatorQueries
{
    Task<CoordinatorDto?> GetByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CoordinatorDto>> SearchAsync(CoordinatorFilter filter, CancellationToken cancellationToken);
}