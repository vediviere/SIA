using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Academic;
using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Contracts.Academic.Responses;
using SIA.AdminBff.Contracts.AcademicStaff.Responses;
using SIA.AdminBff.Infrastructure.Errors;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Controllers.Academic;

[ApiController]
[Authorize]
[Route("api/academic-planning")]
public sealed class AcademicPlanningController : ControllerBase
{
  private readonly IAcademicClient _academicClient;
  private readonly ISchedulingClient _schedulingClient;
  private readonly ITenantContext _tenantContext;

  public AcademicPlanningController(IAcademicClient academicClient, ISchedulingClient schedulingClient, ITenantContext tenantContext)
  {
    _academicClient = academicClient;
    _schedulingClient = schedulingClient;
    _tenantContext = tenantContext;
  }

  [HttpGet("context/{educationalProgramId:guid}")]
  [ProducesResponseType(typeof(AcademicPlanningContextResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status502BadGateway)]
  [ProducesResponseType(typeof(BffErrorResponse), StatusCodes.Status503ServiceUnavailable)]
  public async Task<ActionResult<AcademicPlanningContextResponse>> GetContextAsync([FromRoute] Guid educationalProgramId, CancellationToken cancellationToken)
  {
    var tenantId = _tenantContext.TenantId;
    var academicContextTask = _academicClient.GetAcademicContextAsync(tenantId, educationalProgramId, cancellationToken);
    var teacherCandidatesTask = _schedulingClient.GetTeacherCandidatesAsync(tenantId, educationalProgramId, cancellationToken);

    await Task.WhenAll(academicContextTask, teacherCandidatesTask);

    var academicContext = await academicContextTask;
    var teacherCandidates = await teacherCandidatesTask;

    return Ok(new AcademicPlanningContextResponse
    {
      AcademicPeriod = MapAcademicPeriod(academicContext.AcademicPeriod),
      EducationalProgram = MapEducationalProgram(academicContext.EducationalProgram),
      StudyPlan = MapStudyPlan(academicContext.StudyPlan),
      Subjects = academicContext.Subjects.Select(MapSubject).ToList(),
      TeacherCandidates = teacherCandidates.Select(MapTeacherCandidate).ToList(),
      IsWithinPlanningWindow = academicContext.IsWithinPlanningWindow
    });
  }

  private static AcademicPeriodResponse MapAcademicPeriod(AcademicPeriodDto source)
  {
    return new AcademicPeriodResponse
    {
      Id = source.Id,
      Code = source.Code,
      Name = source.Name,
      IsActive = source.Status,
      PlanningStartDate = source.AcademicLoadProcessStartDate,
      PlanningEndDate = source.AcademicLoadProcessEndDate
    };
  }

  private static EducationalProgramResponse MapEducationalProgram(EducationalProgramDto source)
  {
    return new EducationalProgramResponse
    {
      Id = source.Id,
      Code = source.Code,
      Name = source.Name,
      Level = source.Level
    };
  }

  private static StudyPlanResponse MapStudyPlan(StudyPlanDto source)
  {
    return new StudyPlanResponse
    {
      Id = source.Id,
      EducationalProgramId = source.EducationalProgramId,
      Code = source.Code,
      Name = source.Name,
      Version = source.Version,
      EffectiveFrom = source.EffectiveFrom,
      IsActive = source.Status
    };
  }

  private static SubjectResponse MapSubject(SubjectDto source)
  {
    return new SubjectResponse
    {
      Id = source.Id,
      Code = source.Code,
      Name = source.Name,
      Semester = source.Semester,
      Credits = source.Credits,
      IsRequired = source.IsRequired
    };
  }

  private static TeacherCandidateResponse MapTeacherCandidate(TeacherCandidateDto source)
  {
    return new TeacherCandidateResponse
    {
      TeacherId = source.TeacherId,
      ProfessionalProfile = source.ProfessionalProfile,
      ProgramId = source.ProgramId,
      ContractHours = source.ContractHours,
      IsActive = source.Status
    };
  }
}
