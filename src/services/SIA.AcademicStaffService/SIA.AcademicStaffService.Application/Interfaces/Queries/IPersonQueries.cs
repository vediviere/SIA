using SIA.AcademicStaffService.Application.DTOs.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Application.Interfaces.Queries;

public interface IPersonQueries
{
    Task<Person?> GetByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Person>> SearchAsync(PersonFilter filter, CancellationToken cancellationToken);
}