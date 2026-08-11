using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.DTOs.ClassroomTypes;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/classroom-types")]
public sealed class ClassroomTypesController : ControllerBase
{
    private readonly CreateClassroomTypeUseCase _createClassroomTypeUseCase;
    private readonly UpdateClassroomTypeUseCase _updateClassroomTypeUseCase;
    private readonly SoftDeleteClassroomTypeUseCase _softDeleteClassroomTypeUseCase;
    private readonly RestoreClassroomTypeUseCase _restoreClassroomTypeUseCase;
    private readonly IClassroomTypeQueries _classroomTypeQueries;

    public ClassroomTypesController(
        CreateClassroomTypeUseCase createClassroomTypeUseCase,
        UpdateClassroomTypeUseCase updateClassroomTypeUseCase,
        SoftDeleteClassroomTypeUseCase softDeleteClassroomTypeUseCase,
        RestoreClassroomTypeUseCase restoreClassroomTypeUseCase,
        IClassroomTypeQueries classroomTypeQueries)
    {
        _createClassroomTypeUseCase = createClassroomTypeUseCase;
        _updateClassroomTypeUseCase = updateClassroomTypeUseCase;
        _softDeleteClassroomTypeUseCase = softDeleteClassroomTypeUseCase;
        _restoreClassroomTypeUseCase = restoreClassroomTypeUseCase;
        _classroomTypeQueries = classroomTypeQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClassroomType>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassroomType>>> SearchAsync([FromQuery] ClassroomTypeFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new ClassroomTypeFilter
        {
            TenantId = filter.TenantId,
            Name = filter.Name,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var classroomTypes = await _classroomTypeQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(classroomTypes);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(ClassroomType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassroomType>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassroomTypeResponse>> CreateAsync([FromBody] CreateClassroomTypeRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassroomTypeUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassroomTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassroomTypeResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateClassroomTypeRequest request, CancellationToken cancellationToken)
    {
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

    [HttpDelete("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _softDeleteClassroomTypeUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
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