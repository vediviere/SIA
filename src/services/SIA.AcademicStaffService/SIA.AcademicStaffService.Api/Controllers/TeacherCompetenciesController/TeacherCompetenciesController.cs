using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.Interfaces;
using SIA.AcademicStaffService.Application.UseCases.TeacherCompetencies;
using SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency;
using SIA.AcademicStaffService.Contracts.Responses.TeacherCompetency;

namespace SIA.AcademicStaffService.Api.Controllers.TeacherCompetenciesController
{
    [Authorize]
    [ApiController]
    [Route("api/teachers/{teacherId:guid}/competencies")]
    public sealed class TeacherCompetenciesController : ControllerBase
    {
        private readonly CreateTeacherCompetencyUseCase _createUseCase;
        private readonly UpdateTeacherCompetencyUseCase _updateUseCase;
        private readonly ActivateTeacherCompetencyUseCase _activateUseCase;
        private readonly DeactivateTeacherCompetencyUseCase _deactivateUseCase;
        private readonly ITenantContext _tenantContext;

        public TeacherCompetenciesController(
            CreateTeacherCompetencyUseCase createUseCase,
            UpdateTeacherCompetencyUseCase updateUseCase,
            ActivateTeacherCompetencyUseCase activateUseCase,
            DeactivateTeacherCompetencyUseCase deactivateUseCase,
            ITenantContext tenantContext)
        {
            _createUseCase = createUseCase;
            _updateUseCase = updateUseCase;
            _activateUseCase = activateUseCase;
            _deactivateUseCase = deactivateUseCase;
            _tenantContext = tenantContext;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateTeacherCompetencyResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateTeacherCompetencyResponse>> CreateAsync(
            [FromRoute] Guid teacherId,
            [FromBody] CreateTeacherCompetencyRequest request,
            CancellationToken cancellationToken)
        {
            var correlationId = ResolveCorrelationId();
            var tenantId = _tenantContext.TenantId;

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            var response = await _createUseCase.ExecuteAsync(tenantId, teacherId, request, correlationId, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateTeacherCompetencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateTeacherCompetencyResponse>> UpdateAsync(
            [FromRoute] Guid teacherId,
            [FromRoute(Name = "id")] Guid teacherCompetencyId,
            [FromBody] UpdateTeacherCompetencyRequest request,
            CancellationToken cancellationToken)
        {
            var correlationId = ResolveCorrelationId();
            var tenantId = _tenantContext.TenantId;

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            var response = await _updateUseCase.ExecuteAsync(
                tenantId,
                teacherCompetencyId,
                request,
                correlationId,
                cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateAsync(
            [FromRoute] Guid teacherId,
            [FromRoute(Name = "id")] Guid teacherCompetencyId,
            CancellationToken cancellationToken)
        {
            var correlationId = ResolveCorrelationId();
            var tenantId = _tenantContext.TenantId;

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            await _activateUseCase.ExecuteAsync(tenantId, teacherCompetencyId, correlationId, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateAsync(
            [FromRoute] Guid teacherId,
            [FromRoute(Name = "id")] Guid teacherCompetencyId,
            CancellationToken cancellationToken)
        {
            var correlationId = ResolveCorrelationId();
            var tenantId = _tenantContext.TenantId;

            Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            await _deactivateUseCase.ExecuteAsync(tenantId, teacherCompetencyId, correlationId, cancellationToken);

            return NoContent();
        }

        private Guid ResolveCorrelationId()
        {
            const string headerName = "X-Correlation-Id";

            if (Request.Headers.TryGetValue(headerName, out var headerValue) && Guid.TryParse(headerValue.FirstOrDefault(), out var correlationId))
            {
                return correlationId;
            }

            return Guid.NewGuid();
        }
    }
}