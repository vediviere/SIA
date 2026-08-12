using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.DTOs.Group;
using SIA.SchedulingService.Application.Common.Exceptions;
namespace SIA.SchedulingService.Contracts.IntegrationEvents.Group;

public sealed class GetGroupByIdUseCase
{
    private readonly IGroupQueries _queries;

    public GetGroupByIdUseCase(IGroupQueries queries)
    {
        _queries = queries;
    }

    public async Task<GroupDto?> ExecuteAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken)
    {
        var group = await _queries.GetByIdAsync(tenantId, groupId, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(groupId);
        }

        return new GroupDto
        {
            Id = group.Id,
            TenantId = tenantId,
            EducationalProgramId = group.EducationalProgramId,
            GroupName = group.GroupName,
            Shift = group.Shift,
            Capacity = group.Capacity,
            Status = group.Status,
            CreatedAtUtc = group.CreatedAtUtc,
            UpdatedAtUtc = group.UpdatedAtUtc,

        };
    }
}