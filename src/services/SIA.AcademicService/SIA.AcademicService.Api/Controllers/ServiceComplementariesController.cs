using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.DTOs.ServiceComplementaries;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.ServiceComplementaries;
using SIA.AcademicService.Contracts.Requests.ServiceComplementaries;
using SIA.AcademicService.Contracts.Responses.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Api.Controllers;

[ApiController]
[Route("api/service-complementaries")]
public sealed class ServiceComplementariesController : ControllerBase
{
    private readonly CreateServiceComplementaryUseCase _createUseCase;
    private readonly UpdateServiceComplementaryUseCase _updateUseCase;
    private readonly SoftDeleteServiceComplementaryUseCase _softDeleteUseCase;
    private readonly RestoreServiceComplementaryUseCase _restoreUseCase;
    private readonly IServiceComplementaryQueries _queries;

    public ServiceComplementariesController(
        CreateServiceComplementaryUseCase createUseCase,
        UpdateServiceComplementaryUseCase updateUseCase,
        SoftDeleteServiceComplementaryUseCase softDeleteUseCase,
        RestoreServiceComplementaryUseCase restoreUseCase,
        IServiceComplementaryQueries queries)
    {
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _softDeleteUseCase = softDeleteUseCase;
        _restoreUseCase = restoreUseCase;
        _queries = queries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ServiceComplementary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ServiceComplementary>>> SearchAsync([FromQuery] ServiceComplementaryFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new ServiceComplementaryFilter
        {
            TenantId = filter.TenantId,
            StudyPlanId = filter.StudyPlanId,
            Type = filter.Type,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var results = await _queries.SearchAsync(secureFilter, cancellationToken);
        return Ok(results);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(ServiceComplementary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceComplementary>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(tenantId, id, cancellationToken);

        if (result == null)
        {
            throw new ServiceComplementaryNotFoundException(id);
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateServiceComplementaryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateServiceComplementaryResponse>> CreateAsync([FromBody] CreateServiceComplementaryRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _createUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdateServiceComplementaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateServiceComplementaryResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdateServiceComplementaryRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updateUseCase.ExecuteAsync(
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

        await _softDeleteUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _restoreUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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