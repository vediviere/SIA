using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.DTOs.AcademicLoadGroups;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadGroups;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadGroups;

namespace SIA.SchedulingService.Api.Controllers.Academic;

[Authorize]
[ApiController]
[Route("api/[AcademicLoadGroups]")]

public sealed class AcademicLoadGroupsController : ControllerBase
{
    private readonly AssignTutorUseCase _assignTutorUseCase;
    private readonly RemoveTutorUseCase _removeTutorUseCase;
    private readonly ReplaceTutorUseCase _replaceTutorUseCase;
    private readonly IAcademicLoadGroupQueries _academicLoadGroupQueries;
    private readonly ITenantContext _tenantContext;

    public AcademicLoadGroupsController(
        AssignTutorUseCase assignTutorUseCase,
        RemoveTutorUseCase removeTutorUseCase,
        ReplaceTutorUseCase replaceTutorUseCase,
        IAcademicLoadGroupQueries academicLoadGroupQueries,
        ITenantContext tenantContext)
    {
        _assignTutorUseCase = assignTutorUseCase;
        _removeTutorUseCase = removeTutorUseCase;
        _replaceTutorUseCase = replaceTutorUseCase;
        _academicLoadGroupQueries = academicLoadGroupQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("group/{groupId:guid}")]
    [ProducesResponseType(typeof(AcademicLoadGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicLoadGroupDto>> GetByGroupIdAsync([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var response = await _academicLoadGroupQueries.GetByGroupIdAsync(tenantId, groupId, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }
        var dto = new AcademicLoadGroupDto
        {
            Id = response.Id,
            TenantId = response.TenantId,
            AcademicLoadId = response.AcademicLoadId,
            GroupId = response.GroupId,
            Hours = response.Hours
        };
        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AssignTutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AssignTutorResponse>> AssignTutorAsync([FromBody] AssignTutorRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _assignTutorUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("replace")]
    [ProducesResponseType(typeof(AssignTutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Si no hay asignación para reemplazar
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AssignTutorResponse>> ReplaceTutorAsync([FromBody] ReplaceTutorRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _replaceTutorUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("group/{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Si no hay asignación para retirar
    public async Task<IActionResult> RemoveTutorAsync([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var request = new RemoveTutorRequest { GroupId = groupId };
        await _removeTutorUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);
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