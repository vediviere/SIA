using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.UseCases.Teachers;
using SIA.SchedulingService.Contracts.Requests.Teachers;
using SIA.SchedulingService.Contracts.Responses.Teachers;

namespace SIA.SchedulingService.Api.Controllers.Teacher;

[Authorize]
[ApiController]
[Route("api/teacher-eligibility")]
public sealed class TeacherEligibilityController : ControllerBase
{
    private readonly ValidateTeacherEligibilityUseCase _validateTeacherEligibilityUseCase;
    private readonly ITenantContext _tenantContext;

    public TeacherEligibilityController(
        ValidateTeacherEligibilityUseCase validateTeacherEligibilityUseCase,
        ITenantContext tenantContext)
    {
        _validateTeacherEligibilityUseCase = validateTeacherEligibilityUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidateTeacherEligibilityResponse>> ValidateAsync(
        [FromBody] ValidateTeacherEligibilityRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var response = await _validateTeacherEligibilityUseCase.ExecuteAsync(tenantId, request, cancellationToken);
        return Ok(response);
    }
}