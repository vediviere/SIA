using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.ExternalServices;
using SIA.SchedulingService.Application.UseCases.Teachers;

namespace SIA.SchedulingService.Api.Controllers.Teacher;

[Authorize]
[ApiController]
[Route("api/teacher-candidates")]
public sealed class TeacherCandidatesController : ControllerBase
{
    private readonly GetCandidateTeachersUseCase _getCandidateTeachersUseCase;
    private readonly ITenantContext _tenantContext;

    public TeacherCandidatesController(
        GetCandidateTeachersUseCase getCandidateTeachersUseCase,
        ITenantContext tenantContext)
    {
        _getCandidateTeachersUseCase = getCandidateTeachersUseCase;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CandidateTeacherDto>>> GetAsync(
        [FromQuery] Guid? programId,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var candidates = await _getCandidateTeachersUseCase.ExecuteAsync(tenantId, programId, cancellationToken);
        return Ok(candidates);
    }
}