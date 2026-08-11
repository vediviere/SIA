using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.DTOs.Classrooms;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/classrooms")]
public sealed class ClassroomsController : ControllerBase
{
    private readonly CreateClassroomUseCase _createClassroomUseCase;
    private readonly UpdateClassroomUseCase _updateClassroomUseCase;
    private readonly SoftDeleteClassroomUseCase _softDeleteClassroomUseCase;
    private readonly RestoreClassroomUseCase _restoreClassroomUseCase;
    private readonly IClassroomQueries _classroomQueries;

    public ClassroomsController(
        CreateClassroomUseCase createClassroomUseCase,
        UpdateClassroomUseCase updateClassroomUseCase,
        SoftDeleteClassroomUseCase softDeleteClassroomUseCase,
        RestoreClassroomUseCase restoreClassroomUseCase,
        IClassroomQueries classroomQueries)
    {
        _createClassroomUseCase = createClassroomUseCase;
        _updateClassroomUseCase = updateClassroomUseCase;
        _softDeleteClassroomUseCase = softDeleteClassroomUseCase;
        _restoreClassroomUseCase = restoreClassroomUseCase;
        _classroomQueries = classroomQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Classroom>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Classroom>>> SearchAsync([FromQuery] ClassroomFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new ClassroomFilter
        {
            TenantId = filter.TenantId,
            BuildingId = filter.BuildingId,
            ClassroomTypeId = filter.ClassroomTypeId,
            Code = filter.Code,
            Name = filter.Name,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var classrooms = await _classroomQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(classrooms);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(Classroom), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Classroom>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var classroom = await _classroomQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (classroom == null)
        {
            throw new ClassroomNotFoundException(id);
        }

        return Ok(classroom);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClassroomResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassroomResponse>> CreateAsync([FromBody] CreateClassroomRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassroomUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassroomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassroomResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateClassroomRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateClassroomUseCase.ExecuteAsync(
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

        await _softDeleteClassroomUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _restoreClassroomUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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