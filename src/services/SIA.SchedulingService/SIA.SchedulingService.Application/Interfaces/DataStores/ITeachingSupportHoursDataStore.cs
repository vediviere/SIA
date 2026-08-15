using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface ITeachingSupportHoursDataStore
{
    Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid activityId, Guid academicLoadId, CancellationToken cancellationToken);
    Task AddTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursCreatedIntegrationEvent integrationEvent,  CancellationToken cancellationToken);
    Task<TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task UpdateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task DeactivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    Task ActivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}