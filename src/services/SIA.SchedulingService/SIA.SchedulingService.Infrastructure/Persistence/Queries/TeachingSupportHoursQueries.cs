using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class TeachingSupportHoursQueries : ITeachingSupportHoursQueries
{
  private readonly SchedulingDbContext _dbContext;

  public TeachingSupportHoursQueries(SchedulingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Domain.Entities.TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
  {
    return _dbContext.TeachingSupportHours.AsNoTracking().FirstOrDefaultAsync(hours => hours.TenantId == tenantId && hours.Id == id, cancellationToken);
  }

  public async Task<int> GetTotalSupportHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedSupportHourId, CancellationToken cancellationToken)
  {
    var query = _dbContext.TeachingSupportHours
        .AsNoTracking()
        .Where(entry =>
            entry.TenantId == tenantId &&
            entry.AcademicLoadId == academicLoadId &&
            entry.Status);

    if (excludedSupportHourId.HasValue)
    {
      query = query.Where(entry => entry.Id != excludedSupportHourId.Value);
    }

    return await query.SumAsync(entry => (int?)entry.Hours, cancellationToken) ?? 0;
  }
}
