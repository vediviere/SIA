using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.DTOs.ClassSchedules;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/class-schedules")]
public sealed class ClassSchedulesController : ControllerBase
{
    private readonly CreateClassScheduleUseCase _createClassScheduleUseCase;
    private readonly UpdateClassScheduleUseCase _updateClassScheduleUseCase;
    private readonly SoftDeleteClassScheduleUseCase _softDeleteClassScheduleUseCase;
    private readonly RestoreClassScheduleUseCase _restoreClassScheduleUseCase;
    private readonly IClassScheduleQueries _classScheduleQueries;

    public ClassSchedulesController(
        CreateClassScheduleUseCase createClassScheduleUseCase,
        UpdateClassScheduleUseCase updateClassScheduleUseCase,
        SoftDeleteClassScheduleUseCase softDeleteClassScheduleUseCase,
        RestoreClassScheduleUseCase restoreClassScheduleUseCase,
        IClassScheduleQueries classScheduleQueries)
    {
        _createClassScheduleUseCase = createClassScheduleUseCase;
        _updateClassScheduleUseCase = updateClassScheduleUseCase;
        _softDeleteClassScheduleUseCase = softDeleteClassScheduleUseCase;
        _restoreClassScheduleUseCase = restoreClassScheduleUseCase;
        _classScheduleQueries = classScheduleQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClassSchedule>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassSchedule>>> SearchAsync([FromQuery] ClassScheduleFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new ClassScheduleFilter
        {
            TenantId = filter.TenantId,
            OfferingId = filter.OfferingId,
            ClassroomLabId = filter.ClassroomLabId,
            AcademicPeriodId = filter.AcademicPeriodId,
            Day = filter.Day,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var classSchedules = await _classScheduleQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(classSchedules);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(ClassSchedule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassSchedule>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var classSchedule = await _classScheduleQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (classSchedule == null)
        {
            throw new ClassScheduleNotFoundException(id);
        }

        return Ok(classSchedule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClassScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassScheduleResponse>> CreateAsync([FromBody] CreateClassScheduleRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassScheduleUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassScheduleResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateClassScheduleRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateClassScheduleUseCase.ExecuteAsync(
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

        await _softDeleteClassScheduleUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _restoreClassScheduleUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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