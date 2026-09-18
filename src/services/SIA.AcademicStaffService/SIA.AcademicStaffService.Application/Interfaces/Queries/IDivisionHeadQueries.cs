using SIA.AcademicStaffService.Application.DTOs.DivisionHeads;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface IDivisionHeadQueries
{
    Task<DivisionHead?> GetByIdAsync(Guid tenantId, Guid divisionHeadId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DivisionHead>> SearchAsync(DivisionHeadFilter filter, CancellationToken cancellationToken);
}