using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;
using SIA.SchedulingService.Application.DTOs.SupportActivity;
using SIA.SchedulingService.Application.Interfaces;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Contracts.Requests.SupportActivity;
using SIA.SchedulingService.Contracts.Responses.SupportActivity;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Api.Controllers.Support;

[Authorize]
[ApiController]
[Route("api/support-activities")]
public sealed class SupportActivitiesController : ControllerBase
{
    private readonly CreateSupportActivityUseCase _createSupportActivityUseCase;
    private readonly UpdateSupportActivityUseCase _updateSupportActivityUseCase;
    private readonly SoftDeleteSupportActivityUseCase _softDeleteSupportActivityUseCase;
    private readonly RestoreSupportActivityUseCase _restoreSupportActivityUseCase;
    private readonly ISupportActivityQueries _supportActivityQueries;
    private readonly ITenantContext _tenantContext;

    public SupportActivitiesController(
        CreateSupportActivityUseCase createSupportActivityUseCase,
        UpdateSupportActivityUseCase updateSupportActivityUseCase,
        SoftDeleteSupportActivityUseCase softDeleteSupportActivityUseCase,
        RestoreSupportActivityUseCase restoreSupportActivityUseCase,
        ISupportActivityQueries supportActivityQueries,
        ITenantContext tenantContext)
    {
        _createSupportActivityUseCase = createSupportActivityUseCase;
        _updateSupportActivityUseCase = updateSupportActivityUseCase;
        _softDeleteSupportActivityUseCase = softDeleteSupportActivityUseCase;
        _restoreSupportActivityUseCase = restoreSupportActivityUseCase;
        _supportActivityQueries = supportActivityQueries;
        _tenantContext = tenantContext;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SupportActivity>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SupportActivity>>> SearchAsync([FromQuery] SupportActivityFilter filter, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var secureFilter = new SupportActivityFilter
        {
            TenantId = tenantId,
            Activity = filter.Activity,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var supportActivities = await _supportActivityQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(supportActivities);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupportActivity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupportActivity>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var supportActivity = await _supportActivityQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (supportActivity == null)
        {
            throw new SupportActivityNotFoundException(id);
        }

        return Ok(supportActivity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSupportActivityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateSupportActivityResponse>> CreateAsync([FromBody] CreateSupportActivityRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createSupportActivityUseCase.ExecuteAsync(tenantId, request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateSupportActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateSupportActivityResponse>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateSupportActivityRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var correlationId = ResolveCorrelationId();
        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateSupportActivityUseCase.ExecuteAsync(
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

        await _softDeleteSupportActivityUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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

        await _restoreSupportActivityUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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