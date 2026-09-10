using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface ITeachingSupportHoursQueries
{
  Task<TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
  Task<int> GetTotalSupportHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedSupportHourId, CancellationToken cancellationToken);
}
