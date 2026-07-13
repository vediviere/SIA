using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.Interfaces;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;

namespace SIA.SchoolControlService.Infrastructure.Persistence.DataStores;

public sealed class SchoolControlDataStore
    : ISchoolControlDataStore
{
  private readonly SchoolControlDbContext _dbContext;

  public SchoolControlDataStore(SchoolControlDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<SubjectReference?> GetSubjectReferenceAsync(Guid subjectId, CancellationToken cancellationToken)
  {
    return _dbContext.SubjectReferences
        .AsNoTracking()
        .FirstOrDefaultAsync(subject => subject.SubjectId == subjectId, cancellationToken);
  }
}
