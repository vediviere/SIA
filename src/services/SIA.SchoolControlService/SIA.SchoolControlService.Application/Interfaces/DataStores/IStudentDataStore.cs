using SIA.SchoolControlService.Domain.Entities;

namespace SIA.SchoolControlService.Application.Interfaces.DataStores;

public interface IStudentDataStore
{
  Task<bool> ExistsByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken);
  Task<Student?> GetByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken);
  Task AddAsync(Student student, CancellationToken cancellationToken);
  Task<bool> TryLinkUserAsync(Student student, Guid userId, CancellationToken cancellationToken);
}
