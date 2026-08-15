using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.DTOs.TeachingSupportHours;
using SIA.SchedulingService.Application.Interfaces.Queries;

namespace SIA.SchedulingService.Application.UseCases.TeachingSupportHours;

public sealed class GetTeachingSupportHoursByIdUseCase
{
    private readonly ITeachingSupportHoursQueries _queries;

    public GetTeachingSupportHoursByIdUseCase(ITeachingSupportHoursQueries queries)
    {
        _queries = queries;
    }

    public async Task<TeachingSupportHoursDto> ExecuteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        var teachingSupportHours = await _queries.GetByIdAsync(tenantId, id, cancellationToken);

        if (teachingSupportHours is null)
        {
            throw new TeachingSupportHoursNotFoundException(id);
        }

        return new TeachingSupportHoursDto
        {
            Id = teachingSupportHours.Id,
            TenantId = teachingSupportHours.TenantId,
            ActivityId = teachingSupportHours.ActivityId,
            AcademicLoadId = teachingSupportHours.AcademicLoadId,
            Hours = teachingSupportHours.Hours,
            Status = teachingSupportHours.Status,
            CreatedAtUtc = teachingSupportHours.CreatedAtUtc,
            UpdatedAtUtc = teachingSupportHours.UpdatedAtUtc
        };
    }
}