using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.Interfaces.Queries;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;

namespace SIA.SchoolControlService.Infrastructure.Persistence.Queries;

public sealed class StudentQueries : IStudentQueries
{
  private readonly SchoolControlDbContext _dbContext;

  public StudentQueries(SchoolControlDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Student?> GetByStudentNumberAsync(Guid tenantId, string studentNumber, CancellationToken cancellationToken)
  {
    return _dbContext.Students
      .AsNoTracking()
      .FirstOrDefaultAsync(
        student => student.TenantId == tenantId && student.StudentNumber == studentNumber,
        cancellationToken);
  }
}
