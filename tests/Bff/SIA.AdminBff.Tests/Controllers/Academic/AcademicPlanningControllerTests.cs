using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Academic;
using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Contracts.Academic.Responses;
using SIA.AdminBff.Controllers.Academic;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Tests.Controllers.Academic;

public sealed class AcademicPlanningControllerTests
{
  [Fact]
  public async Task GetContextAsync_WithValidData_ShouldComposeResponse()
  {
    var tenantId = Guid.NewGuid();
    var programId = Guid.NewGuid();
    var periodId = Guid.NewGuid();
    var teacherId = Guid.NewGuid();

    var academicClient = new AcademicClientFake(new AcademicContextDto
    {
      AcademicPeriod = new AcademicPeriodDto
      {
        Id = periodId,
        Code = "2026-1",
        Name = "Periodo 2026-1",
        Status = true,
        AcademicLoadProcessStartDate = new DateOnly(2026, 8, 1),
        AcademicLoadProcessEndDate = new DateOnly(2026, 9, 30)
      },
      EducationalProgram = new EducationalProgramDto
      {
        Id = programId,
        Code = "ISC",
        Name = "Ingeniería en Sistemas",
        Level = "Licenciatura"
      },
      StudyPlan = new StudyPlanDto
      {
        Id = Guid.NewGuid(),
        EducationalProgramId = programId,
        Code = "ISC-2026",
        Name = "Plan ISC 2026",
        Version = "1",
        EffectiveFrom = new DateOnly(2026, 1, 1),
        Status = true
      },
      Subjects =
        [
            new SubjectDto
                {
                    Id = Guid.NewGuid(),
                    Code = "SIA-101",
                    Name = "Arquitectura de Software",
                    Semester = 5,
                    Credits = 5,
                    IsRequired = true
                }
        ],
      IsWithinPlanningWindow = true
    });

    var schedulingClient = new SchedulingClientFake(
    [
        new TeacherCandidateDto
            {
                TeacherId = teacherId,
                ProfessionalProfile = "Desarrollo de software",
                ProgramId = programId,
                ContractHours = 40,
                Status = true
            }
    ]);

    var controller = new AcademicPlanningController(academicClient, schedulingClient, new TenantContextFake(tenantId));

    var result = await controller.GetContextAsync(programId, CancellationToken.None);

    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var response = Assert.IsType<AcademicPlanningContextResponse>(okResult.Value);

    Assert.Equal(periodId, response.AcademicPeriod.Id);
    Assert.Equal(programId, response.EducationalProgram.Id);
    Assert.Single(response.Subjects);
    Assert.Single(response.TeacherCandidates);
    Assert.Equal(teacherId, response.TeacherCandidates.Single().TeacherId);
    Assert.True(response.IsWithinPlanningWindow);
    Assert.Equal(tenantId, academicClient.TenantId);
    Assert.Equal(tenantId, schedulingClient.TenantId);
    Assert.Equal(programId, academicClient.ProgramId);
    Assert.Equal(programId, schedulingClient.ProgramId);
  }

  private sealed class TenantContextFake : ITenantContext
  {
    public TenantContextFake(Guid tenantId)
    {
      TenantId = tenantId;
    }

    public Guid TenantId { get; }
  }

  private sealed class AcademicClientFake : IAcademicClient
  {
    private readonly AcademicContextDto _response;

    public AcademicClientFake(AcademicContextDto response)
    {
      _response = response;
    }

    public Guid TenantId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Task<AcademicContextDto> GetAcademicContextAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
    {
      TenantId = tenantId;
      ProgramId = educationalProgramId;

      return Task.FromResult(_response);
    }
  }

  private sealed class SchedulingClientFake : ISchedulingClient
  {
    private readonly IReadOnlyCollection<TeacherCandidateDto> _teachers;

    public SchedulingClientFake(IReadOnlyCollection<TeacherCandidateDto> teachers)
    {
      _teachers = teachers;
    }

    public Guid TenantId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Task<IReadOnlyCollection<TeacherCandidateDto>> GetTeacherCandidatesAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
    {
      TenantId = tenantId;
      ProgramId = educationalProgramId;

      return Task.FromResult(_teachers);
    }

    public Task<ProposalDto> CreateProposalAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    public Task<ProposalDto> SubmitForReviewAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    public Task<LoadDto> CreateLoadAsync(LoadCreateDto request, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    public Task<LoadDto> UpdateLoadAsync(Guid tenantId, Guid loadId, LoadUpdateDto request, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    public Task<SupportHourDto> CreateSupportHourAsync(SupportHourCreateDto request, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    public Task<SupportHourDto> UpdateSupportHourAsync(Guid tenantId, Guid supportHourId, SupportHourUpdateDto request, CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }
  }
}
