using SIA.SchedulingService.Application.DTOs.SupportActivity;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface ISupportActivityQueries
{
    Task<SupportActivity?> GetByIdAsync(Guid tenantId, Guid supportActivityId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SupportActivity>> SearchAsync(SupportActivityFilter filter, CancellationToken cancellationToken);
}