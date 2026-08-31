using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.Common.Services.AcademicLoads;

public sealed class AcademicLoadClassHoursCalculator
{
  private readonly IAcademicOfferingQueries _queries;

  public AcademicLoadClassHoursCalculator(IAcademicOfferingQueries queries)
  {
    _queries = queries;
  }

  public async Task RecalculateAsync(AcademicLoad academicLoad, AcademicOffering academicOffering, CancellationToken cancellationToken)
  {
    var totalClassHours = await _queries.GetTotalClassHoursByAcademicLoadAsync(
        academicLoad.TenantId,
        academicLoad.Id,
        academicOffering.Id,
        cancellationToken);

    if (academicOffering.Status)
    {
      totalClassHours += academicOffering.ClassHours;
    }

    academicLoad.SetClassHours(totalClassHours);
  }
}
