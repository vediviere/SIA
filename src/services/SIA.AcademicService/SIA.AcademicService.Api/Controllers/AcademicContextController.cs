using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Api.Extensions;
using SIA.AcademicService.Application.UseCases.AcademicContext;
using SIA.AcademicService.Contracts.Requests.AcademicContext;
using SIA.AcademicService.Contracts.Responses.AcademicContext;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/academic-context")]
[Authorize]
public class AcademicContextController : ControllerBase
{
    private readonly GetAcademicContextUseCase _useCase;

    public AcademicContextController(GetAcademicContextUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet("educational-programs/{educationalProgramId}")]
    [ProducesResponseType(typeof(GetAcademicContextResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetAcademicContextResponse>> GetAcademicContext(
        [FromRoute] Guid educationalProgramId,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        if (educationalProgramId == Guid.Empty)
        {
            return BadRequest("El EducationalProgramId es requerido.");
        }

        var request = new GetAcademicContextRequest
        {
            EducationalProgramId = educationalProgramId 
        };

        var response = await _useCase.ExecuteAsync(tenantId, request, cancellationToken);
        return Ok(response);
    }
}