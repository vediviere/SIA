using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Common.Services.AcademicLoads;

public sealed class AcademicLoadSupportHoursCalculator
{
  private readonly ITeachingSupportHoursQueries _queries;

  public AcademicLoadSupportHoursCalculator(ITeachingSupportHoursQueries queries)
  {
    _queries = queries;
  }

  public async Task RecalculateAsync(AcademicLoad academicLoad, TeachingSupportHour supportHour, CancellationToken cancellationToken)
  {
    var totalSupportHours = await _queries.GetTotalSupportHoursByAcademicLoadAsync(
      academicLoad.TenantId,
      academicLoad.Id,
      supportHour.Id,
      cancellationToken);

    if (supportHour.Status)
    {
      totalSupportHours += supportHour.Hours;
    }

    academicLoad.SetSupportHours(totalSupportHours);
  }
}
