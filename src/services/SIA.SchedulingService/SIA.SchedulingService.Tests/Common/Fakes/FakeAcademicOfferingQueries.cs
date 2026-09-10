using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeAcademicOfferingQueries : IAcademicOfferingQueries
{
  public AcademicOffering? AcademicOffering { get; set; }
  public int AssignedClassHours { get; set; }
  public int TotalClassHoursByAcademicLoad { get; set; }

  public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken)
  {
    return Task.FromResult(AcademicOffering);
  }

  public Task<int> GetAssignedClassHoursAsync(Guid tenantId, Guid teacherId, Guid academicPeriodId, Guid? excludedOfferingId, CancellationToken cancellationToken)
  {
    return Task.FromResult(AssignedClassHours);
  }

  public Task<int> GetTotalClassHoursByAcademicLoadAsync(Guid tenantId, Guid academicLoadId, Guid? excludedOfferingId, CancellationToken cancellationToken)
  {
    return Task.FromResult(TotalClassHoursByAcademicLoad);
  }
}
