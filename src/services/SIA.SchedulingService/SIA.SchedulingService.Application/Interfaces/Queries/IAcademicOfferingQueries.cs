using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IAcademicOfferingQueries
{
  Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken);
  Task<int> GetAssignedClassHoursAsync(Guid tenantId, Guid teacherId, Guid academicPeriodId, Guid? excludedOfferingId, CancellationToken cancellationToken);
  Task<int> GetTotalClassHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedOfferingId, CancellationToken cancellationToken);
}
