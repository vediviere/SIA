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
    public TeachingSupportHour? AddedTeachingSupportHours { get; private set; }
    public TeachingSupportHoursCreatedIntegrationEvent? AddedCreatedEvent { get; private set; }
    public TeachingSupportHour? UpdatedTeachingSupportHours { get; private set; }
    public TeachingSupportHoursUpdatedIntegrationEvent? AddedUpdatedEvent { get; private set; }
    public TeachingSupportHoursActivatedIntegrationEvent? AddedActivatedEvent { get; private set; }
    public TeachingSupportHoursDeactivatedIntegrationEvent? AddedDeactivatedEvent { get; private set; }

    public Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid activityId, Guid academicLoadId, CancellationToken cancellationToken) => Task.FromResult(ExistsResult);
    public Task<TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken) => Task.FromResult(_tshReturn);
    public Task AddTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedTeachingSupportHours = teachingSupportHours;
        AddedCreatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task UpdateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedTeachingSupportHours = teachingSupportHours;
        AddedUpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task ActivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
    public Task DeactivateTeachingSupportHoursWithOutboxAsync(TeachingSupportHour teachingSupportHours, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedDeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}