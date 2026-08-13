using SIA.SchedulingService.Application.DTOs.SupportSchedules;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface ISupportScheduleQueries
{
    Task<SupportSchedule?> GetByIdAsync(Guid tenantId, Guid supportScheduleId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<SupportSchedule>> SearchAsync(SupportScheduleFilter filter, CancellationToken cancellationToken);
}
