using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;
using SIA.SchedulingService.Application.DTOs.ClassSchedules;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Contracts.Requests.ClassSchedule;
using SIA.SchedulingService.Contracts.Responses.ClassSchedule;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Api.Controllers.Academic;

[Authorize]
[ApiController]
[Route("api/class-schedules")]
public sealed class ClassSchedulesController : ControllerBase
{
    private readonly CreateClassScheduleUseCase _createClassScheduleUseCase;
    private readonly UpdateClassScheduleUseCase _updateClassScheduleUseCase;
    private readonly SoftDeleteClassScheduleUseCase _softDeleteClassScheduleUseCase;
    private readonly RestoreClassScheduleUseCase _restoreClassScheduleUseCase;
    private readonly IClassScheduleQueries _classScheduleQueries;
    private readonly ITenantContext _tenantContext;

    public ClassSchedulesController(
        CreateClassScheduleUseCase createClassScheduleUseCase,
        UpdateClassScheduleUseCase updateClassScheduleUseCase,
        SoftDeleteClassScheduleUseCase softDeleteClassScheduleUseCase,
        RestoreClassScheduleUseCase restoreClassScheduleUseCase,
        IClassScheduleQueries classScheduleQueries,
        ITenantContext tenantContext)
    {
        _createClassScheduleUseCase = createClassScheduleUseCase;
        _updateClassScheduleUseCase = updateClassScheduleUseCase;
        _softDeleteClassScheduleUseCase = softDeleteClassScheduleUseCase;
        _restoreClassScheduleUseCase = restoreClassScheduleUseCase;
        _classScheduleQueries = classScheduleQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClassSchedule>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassSchedule>>> SearchAsync([FromQuery] ClassScheduleFilter filter, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var secureFilter = new ClassScheduleFilter
        {
            TenantId = tenantId,
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassSchedule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassSchedule>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var classSchedule = await _classScheduleQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (classSchedule == null)
        {
            throw new ClassScheduleNotFoundException(id);
        }

        return Ok(classSchedule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClassScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateClassScheduleResponse>> CreateAsync([FromBody] CreateClassScheduleRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createClassScheduleUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateClassScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateClassScheduleResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateClassScheduleRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _softDeleteClassScheduleUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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