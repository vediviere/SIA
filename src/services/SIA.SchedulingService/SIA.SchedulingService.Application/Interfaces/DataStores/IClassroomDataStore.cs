using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

namespace SIA.SchedulingService.Application.Interfaces.DataStores
{
    public interface IClassroomDataStore
    {
        Task<bool> ClassroomCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken);

        Task AddClassroomWithOutboxAsync(Classroom classroom, ClassroomCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task<Classroom?> GetClassroomByIdAsync(Guid tenantId, Guid classroomId, CancellationToken cancellationToken);

        Task UpdateClassroomWithOutboxAsync(Classroom classroom, ClassroomUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task SoftDeleteClassroomWithOutboxAsync(Classroom classroom, ClassroomDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task RestoreClassroomWithOutboxAsync(Classroom classroom, ClassroomRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
