using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Tests.Common.Fakes;

public sealed class FakeTeachingSupportHoursDataStore : ITeachingSupportHoursDataStore
{
    private readonly TeachingSupportHour? _tshReturn;

    public FakeTeachingSupportHoursDataStore(TeachingSupportHour? tshReturn = null)
    {
        _tshReturn = tshReturn;
    }

    public bool ExistsResult { get; set; }
    public bool SupportHoursAdded { get; private set; }
    public bool SupportHoursUpdated { get; private set; }
    public bool SupportHoursActivated { get; private set; }
    public bool SupportHoursDeactivated { get; private set; }

    public Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid activityId, Guid academicLoadId, CancellationToken cancellationToken) => Task.FromResult(ExistsResult);
    public Task<TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken) => Task.FromResult(_tshReturn);
    public Task AddTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        SupportHoursAdded = true;
        return Task.CompletedTask;
    }
    public Task UpdateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        SupportHoursUpdated = true;
        return Task.CompletedTask;
    }
    public Task ActivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        SupportHoursActivated = true;
        return Task.CompletedTask;
    }
    public Task DeactivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        SupportHoursDeactivated = true;
        return Task.CompletedTask;
    }
}