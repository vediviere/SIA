using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.Interfaces.DataStores;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;

namespace SIA.SchoolControlService.Infrastructure.Persistence.DataStores;

public sealed class StudentDataStore : IStudentDataStore
{
  private readonly SchoolControlDbContext _dbContext;

  public StudentDataStore(SchoolControlDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> ExistsByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken)
  {
    return _dbContext.Students.AnyAsync(
      student => student.TenantId == tenantId && student.StudentNumber == studentNumber,
      cancellationToken);
  }

  public async Task AddAsync(Student student, CancellationToken cancellationToken)
  {
    await _dbContext.Students.AddAsync(student, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
