using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class AcademicOfferingQueries : IAcademicOfferingQueries
{
  private readonly SchedulingDbContext _dbContext;

  public AcademicOfferingQueries(SchedulingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicOfferings.AsNoTracking().FirstOrDefaultAsync(offering => offering.TenantId == tenantId && offering.Id == offeringId, cancellationToken);
  }

  public async Task<int> GetAssignedClassHoursAsync(Guid tenantId, Guid teacherId, Guid academicPeriodId, Guid? excludedOfferingId, CancellationToken cancellationToken)
  {
    var assignedHours = await (
        from offering in _dbContext.AcademicOfferings.AsNoTracking()
        join academicLoad in _dbContext.AcademicLoad.AsNoTracking() on offering.AcademicLoadId equals academicLoad.Id
        where offering.TenantId == tenantId
            && academicLoad.TenantId == tenantId
            && academicLoad.TeacherId == teacherId
            && academicLoad.AcademicPeriodId == academicPeriodId
            && offering.Status
            && academicLoad.Status
            && (!excludedOfferingId.HasValue || offering.Id != excludedOfferingId.Value)
        select (int?)offering.ClassHours
    ).SumAsync(cancellationToken);

    return assignedHours ?? 0;
  }

  public async Task<int> GetTotalClassHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedOfferingId, CancellationToken cancellationToken)
  {
    var query = _dbContext.AcademicOfferings
        .AsNoTracking()
        .Where(offering =>
            offering.TenantId == tenantId &&
            offering.AcademicLoadId == academicLoadId &&
            offering.Status);

    if (excludedOfferingId.HasValue)
    {
      query = query.Where(offering => offering.Id != excludedOfferingId.Value);
    }

    return await query.SumAsync(offering => (int?)offering.ClassHours, cancellationToken) ?? 0;
  }
}
