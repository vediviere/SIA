using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

namespace SIA.SchedulingService.Application.Interfaces.DataStores
{
    public interface IClassroomLabDataStore
    {
        Task<bool> ClassroomLabCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

        Task AddClassroomLabWithOutboxAsync(ClassroomLab classroom, ClassroomLabCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task<ClassroomLab?> GetClassroomLabByIdAsync(Guid tenantId, Guid classroomId, CancellationToken cancellationToken);

        Task UpdateClassroomLabWithOutboxAsync(ClassroomLab classroom, ClassroomLabUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task SoftDeleteClassroomLabWithOutboxAsync(ClassroomLab classroom, ClassroomLabDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task RestoreClassroomLabWithOutboxAsync(ClassroomLab classroom, ClassroomLabRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
