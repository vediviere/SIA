using SIA.SchedulingService.Application.DTOs.ClassSchedules;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IClassScheduleQueries
{
    Task<ClassSchedule?> GetByIdAsync(Guid tenantId, Guid classScheduleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ClassSchedule>> SearchAsync(ClassScheduleFilter filter, CancellationToken cancellationToken);
}