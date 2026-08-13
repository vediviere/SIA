using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;
using SIA.SchedulingService.Application.DTOs.SupportSchedules;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Contracts.Requests.SupportSchedules;
using SIA.SchedulingService.Contracts.Responses.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Api.Controllers;

[ApiController]
[Route("api/support-schedules")]
public sealed class SupportSchedulesController : ControllerBase
{
    private readonly CreateSupportScheduleUseCase _createSupportScheduleUseCase;
    private readonly UpdateSupportScheduleUseCase _updateSupportScheduleUseCase;
    private readonly SoftDeleteSupportScheduleUseCase _softDeleteSupportScheduleUseCase;
    private readonly RestoreSupportScheduleUseCase _restoreSupportScheduleUseCase;
    private readonly ISupportScheduleQueries _supportScheduleQueries;

    public SupportSchedulesController(
        CreateSupportScheduleUseCase createSupportScheduleUseCase,
        UpdateSupportScheduleUseCase updateSupportScheduleUseCase,
        SoftDeleteSupportScheduleUseCase softDeleteSupportScheduleUseCase,
        RestoreSupportScheduleUseCase restoreSupportScheduleUseCase,
        ISupportScheduleQueries supportScheduleQueries)
    {
        _createSupportScheduleUseCase = createSupportScheduleUseCase;
        _updateSupportScheduleUseCase = updateSupportScheduleUseCase;
        _softDeleteSupportScheduleUseCase = softDeleteSupportScheduleUseCase;
        _restoreSupportScheduleUseCase = restoreSupportScheduleUseCase;
        _supportScheduleQueries = supportScheduleQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SupportSchedule>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SupportSchedule>>> SearchAsync([FromQuery] SupportScheduleFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new SupportScheduleFilter
        {
            TenantId = filter.TenantId,
            SupportHourId = filter.SupportHourId,
            ClassroomLabId = filter.ClassroomLabId,
            AcademicPeriodId = filter.AcademicPeriodId,
            Day = filter.Day,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var supportSchedules = await _supportScheduleQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(supportSchedules);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(SupportSchedule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupportSchedule>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var supportSchedule = await _supportScheduleQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (supportSchedule == null)
        {
            throw new SupportScheduleNotFoundException(id);
        }

        return Ok(supportSchedule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSupportScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateSupportScheduleResponse>> CreateAsync([FromBody] CreateSupportScheduleRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createSupportScheduleUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateSupportScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateSupportScheduleResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateSupportScheduleRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateSupportScheduleUseCase.ExecuteAsync(
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

        await _softDeleteSupportScheduleUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _restoreSupportScheduleUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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