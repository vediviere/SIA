
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.DTOs.AcademicLoad;
using SIA.SchedulingService.Application.Interfaces.Queries;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoads;

public sealed class GetAcademicLoadByIdUseCase
{
  private readonly IAcademicLoadQueries _queries;

  public GetAcademicLoadByIdUseCase(IAcademicLoadQueries queries)
  {
    _queries = queries;
  }

  public async Task<AcademicLoadDto> ExecuteAsync(Guid tenantId, Guid academicLoadId, CancellationToken cancellationToken)
  {
    var academicLoad = await _queries.GetByIdAsync(tenantId, academicLoadId, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(academicLoadId);
    }
    return new AcademicLoadDto
    {
      Id = academicLoad.Id,
      TenantId = academicLoad.TenantId,
      ProposalId = academicLoad.ProposalId,
      TeacherId = academicLoad.TeacherId,
      DivisionId = academicLoad.DivisionId,
      AcademicPeriodId = academicLoad.AcademicPeriodId,
      OfficialLetterNumber = academicLoad.OfficialLetterNumber,
      ProposedDate = academicLoad.ProposedDate,
      ClassHours = academicLoad.ClassHours,
      SupportHours = academicLoad.SupportHours,
      AssignmentDate = academicLoad.AssignmentDate,
      Status = academicLoad.Status,
      CreatedAtUtc = academicLoad.CreatedAtUtc,
      UpdatedAtUtc = academicLoad.UpdatedAtUtc
    };
  }

}
