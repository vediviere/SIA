using SIA.AcademicStaffService.Application.DTOs.Coordinators;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface ICoordinatorQueries
{
    Task<Coordinator?> GetByIdAsync(Guid tenantId, Guid coordinatorId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Coordinator>> SearchAsync(CoordinatorFilter filter, CancellationToken cancellationToken);
}