using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeTeachingSupportHoursQueries : ITeachingSupportHoursQueries
{
  public TeachingSupportHour? TeachingSupportHour { get; set; }
  public int TotalSupportHoursByAcademicLoad { get; set; }

  public Task<TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
  {
    return Task.FromResult(TeachingSupportHour);
  }

  public Task<int> GetTotalSupportHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedSupportHourId, CancellationToken cancellationToken)
  {
    return Task.FromResult(TotalSupportHoursByAcademicLoad);
  }
}
