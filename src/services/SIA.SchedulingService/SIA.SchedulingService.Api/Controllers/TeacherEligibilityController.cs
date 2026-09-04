using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.UseCases.Teachers;
using SIA.SchedulingService.Contracts.Requests.Teachers;
using SIA.SchedulingService.Contracts.Responses.Teachers;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/teacher-eligibility")]
public sealed class TeacherEligibilityController : ControllerBase
{
    private readonly ValidateTeacherEligibilityUseCase _validateTeacherEligibilityUseCase;

    public TeacherEligibilityController(ValidateTeacherEligibilityUseCase validateTeacherEligibilityUseCase)
    {
        _validateTeacherEligibilityUseCase = validateTeacherEligibilityUseCase;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidateTeacherEligibilityResponse>> ValidateAsync(
        [FromBody] ValidateTeacherEligibilityRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _validateTeacherEligibilityUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }
}