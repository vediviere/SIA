using SIA.SchedulingService.Application.DTOs.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IClassroomTypeQueries
{
    Task<ClassroomType?> GetByIdAsync(Guid tenantId, Guid classroomTypeId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ClassroomType>> SearchAsync(ClassroomTypeFilter filter, CancellationToken cancellationToken);
}