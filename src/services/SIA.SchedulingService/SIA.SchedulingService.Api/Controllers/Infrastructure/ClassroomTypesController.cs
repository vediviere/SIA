using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.DTOs.ClassroomTypes;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Api.Controllers.Infrastructure;

[Authorize]
[ApiController]
[Route("api/classroom-types")]
public sealed class ClassroomTypesController : ControllerBase
{
    private readonly CreateClassroomTypeUseCase _createClassroomTypeUseCase;
    private readonly UpdateClassroomTypeUseCase _updateClassroomTypeUseCase;
    private readonly SoftDeleteClassroomTypeUseCase _softDeleteClassroomTypeUseCase;
    private readonly RestoreClassroomTypeUseCase _restoreClassroomTypeUseCase;
    private readonly IClassroomTypeQueries _classroomTypeQueries;
    private readonly ITenantContext _tenantContext;

    public ClassroomTypesController(
        CreateClassroomTypeUseCase createClassroomTypeUseCase,
        UpdateClassroomTypeUseCase updateClassroomTypeUseCase,
        SoftDeleteClassroomTypeUseCase softDeleteClassroomTypeUseCase,
        RestoreClassroomTypeUseCase restoreClassroomTypeUseCase,
        IClassroomTypeQueries classroomTypeQueries,
        ITenantContext tenantContext)
    {
        _createClassroomTypeUseCase = createClassroomTypeUseCase;
        _updateClassroomTypeUseCase = updateClassroomTypeUseCase;
        _softDeleteClassroomTypeUseCase = softDeleteClassroomTypeUseCase;
        _restoreClassroomTypeUseCase = restoreClassroomTypeUseCase;
        _classroomTypeQueries = classroomTypeQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClassroomType>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassroomType>>> SearchAsync([FromQuery] ClassroomTypeFilter filter, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var secureFilter = new ClassroomTypeFilter
        {
            TenantId = tenantId,
            Name = filter.Name,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var classroomTypes = await _classroomTypeQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(classroomTypes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassroomType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassroomType>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var classroomType = await _classroomTypeQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (classroomType == null)
        {
            throw new ClassroomTypeNotFoundException(id);
        }

        return Ok(classroomType);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClassroomTypeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassroomTypeResponse>> CreateAsync([FromBody] CreateClassroomTypeRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassroomTypeUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassroomTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassroomTypeResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateClassroomTypeRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateClassroomTypeUseCase.ExecuteAsync(
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

        await _softDeleteClassroomTypeUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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

        await _restoreClassroomTypeUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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