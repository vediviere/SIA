using Microsoft.AspNetCore.Mvc;
using SIA.AcademicStaffService.Application.DTOs.Persons;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Contracts.Requests.Persons;
using SIA.AcademicStaffService.Contracts.Responses.Persons;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Api.Controllers;

[ApiController]
[Route("api/persons")]
public sealed class PersonsController : ControllerBase
{
    private readonly CreatePersonUseCase _createPersonUseCase;
    private readonly UpdatePersonUseCase _updatePersonUseCase;
    private readonly ActivatePersonUseCase _activatePersonUseCase;
    private readonly DeactivatePersonUseCase _deactivatePersonUseCase;
    private readonly IPersonQueries _personQueries;

    public PersonsController(
        CreatePersonUseCase createPersonUseCase,
        UpdatePersonUseCase updatePersonUseCase,
        ActivatePersonUseCase activatePersonUseCase,
        DeactivatePersonUseCase deactivatePersonUseCase,
        IPersonQueries personQueries)
    {
        _createPersonUseCase = createPersonUseCase;
        _updatePersonUseCase = updatePersonUseCase;
        _activatePersonUseCase = activatePersonUseCase;
        _deactivatePersonUseCase = deactivatePersonUseCase;
        _personQueries = personQueries;
    }

    [HttpGet("Filter")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Person>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Person>>> SearchAsync([FromQuery] PersonFilter filter, CancellationToken cancellationToken)
    {
        var secureFilter = new PersonFilter
        {
            TenantId = filter.TenantId,
            EmployeeNumber = filter.EmployeeNumber,
            FirstName = filter.FirstName,
            PaternalLastName = filter.PaternalLastName,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var persons = await _personQueries.SearchAsync(secureFilter, cancellationToken);
        return Ok(persons);
    }

    [HttpGet("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Person>> GetByIdAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var person = await _personQueries.GetByIdAsync(tenantId, id, cancellationToken);

        if (person == null)
        {
            return NotFound(new { message = $"No se encontró la persona con Id {id}." });
        }

        return Ok(person);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatePersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePersonResponse>> CreateAsync([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append(
            "X-Correlation-Id",
            correlationId.ToString());

        var response = await _createPersonUseCase.ExecuteAsync(request, correlationId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{tenantId:guid}/{id:guid}")]
    [ProducesResponseType(typeof(UpdatePersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdatePersonResponse>> UpdateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, [FromBody] UpdatePersonRequest request, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        var response = await _updatePersonUseCase.ExecuteAsync(
            tenantId,
            id,
            request,
            correlationId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{tenantId:guid}/{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _activatePersonUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{tenantId:guid}/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var correlationId = ResolveCorrelationId();

        Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

        await _deactivatePersonUseCase.ExecuteAsync(tenantId, id, correlationId, cancellationToken);

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