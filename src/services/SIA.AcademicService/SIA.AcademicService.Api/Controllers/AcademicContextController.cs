using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.UseCases.AcademicContext;
using SIA.AcademicService.Contracts.Requests.AcademicContext;
using SIA.AcademicService.Contracts.Responses.AcademicContext;

namespace SIA.AcademicService.Api.Controllers
{
    [ApiController]
    [Route("api/academic-context")]
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
            [FromHeader(Name = "tenantid")] Guid tenantId,
            [FromRoute] Guid educationalProgramId,
            CancellationToken cancellationToken)
        {
            if (tenantId == Guid.Empty || educationalProgramId == Guid.Empty)
            {
                return BadRequest("El TenantId y el EducationalProgramId son requeridos.");
            }

            var request = new GetAcademicContextRequest
            {
                TenantId = tenantId,
                EducationalProgramId = educationalProgramId
            };

            var response = await _useCase.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}