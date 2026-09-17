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

  public Task<Student?> GetByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken)
  {
    return _dbContext.Students.FirstOrDefaultAsync(
      student => student.TenantId == tenantId && student.StudentNumber == studentNumber,
      cancellationToken);
  }

  public async Task AddAsync(Student student, CancellationToken cancellationToken)
  {
    await _dbContext.Students.AddAsync(student, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<bool> TryLinkUserAsync(Student student, Guid userId, CancellationToken cancellationToken)
  {
    var changed = student.LinkUser(userId);

    if (!changed)
    {
      return true;
    }

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);

      return true;
    }
    catch (DbUpdateConcurrencyException)
    {
      await _dbContext.Entry(student).ReloadAsync(cancellationToken);

      return student.UserId == userId;
    }
  }
}
