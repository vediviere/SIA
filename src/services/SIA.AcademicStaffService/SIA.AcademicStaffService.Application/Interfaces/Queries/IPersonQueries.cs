using SIA.AcademicStaffService.Application.DTOs.Persons;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface IPersonQueries
{
    Task<PersonDto?> GetByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PersonDto>> SearchAsync(PersonFilter filter, CancellationToken cancellationToken);
}