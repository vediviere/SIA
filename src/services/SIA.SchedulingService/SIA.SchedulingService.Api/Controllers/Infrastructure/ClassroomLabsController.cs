using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.DTOs.Classrooms;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Api.Controllers.Infrastructure;

[Authorize]
[ApiController]
[Route("api/classroom-labs")]
public sealed class ClassroomLabsController : ControllerBase
{
    private readonly CreateClassroomLabUseCase _createClassroomLabUseCase;
    private readonly UpdateClassroomLabUseCase _updateClassroomLabUseCase;
    private readonly SoftDeleteClassroomLabUseCase _softDeleteClassroomLabUseCase;
    private readonly RestoreClassroomLabUseCase _restoreClassroomLabUseCase;
    private readonly IClassroomLabQueries _classroomLabQueries;
    private readonly ITenantContext _tenantContext;

    public ClassroomLabsController(
        CreateClassroomLabUseCase createClassroomLabUseCase,
        UpdateClassroomLabUseCase updateClassroomLabUseCase,
        SoftDeleteClassroomLabUseCase softDeleteClassroomLabUseCase,
        RestoreClassroomLabUseCase restoreClassroomLabUseCase,
        IClassroomLabQueries classroomLabQueries,
        ITenantContext tenantContext)
    {
        _createClassroomLabUseCase = createClassroomLabUseCase;
        _updateClassroomLabUseCase = updateClassroomLabUseCase;
        _softDeleteClassroomLabUseCase = softDeleteClassroomLabUseCase;
        _restoreClassroomLabUseCase = restoreClassroomLabUseCase;
        _classroomLabQueries = classroomLabQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClassroomLab>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassroomLab>>> SearchAsync([FromQuery] ClassroomLabFilter filter, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var secureFilter = new ClassroomLabFilter
        {
            TenantId  = tenantId,
            BuildingId = filter.BuildingId,
            ClassroomTypeId = filter.ClassroomTypeId,
            Code = filter.Code,
            Name = filter.Name,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var classroomLabs = await _classroomLabQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(classroomLabs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassroomLab), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassroomLab>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var classroomLab = await _classroomLabQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (classroomLab == null)
        {
            throw new ClassroomLabNotFoundException(id);
        }

        return Ok(classroomLab);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClassroomLabResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassroomLabResponse>> CreateAsync([FromBody] CreateClassroomLabRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassroomLabUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassroomLabResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassroomLabResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateClassroomLabRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateClassroomLabUseCase.ExecuteAsync(
            tenantId,
            id,
            request,
            correlationId,
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _softDeleteClassroomLabUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _restoreClassroomLabUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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