using SIA.SchedulingService.Application.DTOs.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IClassroomLabQueries
{
    Task<ClassroomLab?> GetByIdAsync(Guid tenantId, Guid classroomId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ClassroomLab>> SearchAsync(ClassroomLabFilter filter, CancellationToken cancellationToken);
}