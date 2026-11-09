using SIA.AcademicStaffService.Application.DTOs.DivisionHeads;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface IDivisionHeadQueries
{
    Task<DivisionHeadDto?> GetByIdAsync(Guid tenantId, Guid divisionHeadId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DivisionHeadDto>> SearchAsync(DivisionHeadFilter filter, CancellationToken cancellationToken);
}