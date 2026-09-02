using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Interfaces.ExternalServices;
using SIA.SchedulingService.Application.UseCases.Teachers;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/teacher-candidates")]
public sealed class TeacherCandidatesController : ControllerBase
{
    private readonly GetCandidateTeachersUseCase _getCandidateTeachersUseCase;

    public TeacherCandidatesController(GetCandidateTeachersUseCase getCandidateTeachersUseCase)
    {
        _getCandidateTeachersUseCase = getCandidateTeachersUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CandidateTeacherDto>>> GetAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] Guid? programId,
        CancellationToken cancellationToken)
    {
        var candidates = await _getCandidateTeachersUseCase.ExecuteAsync(tenantId, programId, cancellationToken);
        return Ok(candidates);
    }
}