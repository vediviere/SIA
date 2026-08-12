using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores
{
    public interface IClassroomTypeDataStore
    {
        Task<bool> ClassroomTypeNameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken);

        Task AddClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task<ClassroomType?> GetClassroomTypeByIdAsync(Guid tenantId, Guid classroomTypeId, CancellationToken cancellationToken);

        Task UpdateClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task SoftDeleteClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken);

        Task RestoreClassroomTypeWithOutboxAsync(ClassroomType classroomType, ClassroomTypeRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
