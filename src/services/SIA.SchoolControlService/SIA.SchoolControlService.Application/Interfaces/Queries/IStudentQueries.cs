using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.Interfaces.Queries;

public interface IStudentQueries
{
  Task<Student?> GetByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken);
}
