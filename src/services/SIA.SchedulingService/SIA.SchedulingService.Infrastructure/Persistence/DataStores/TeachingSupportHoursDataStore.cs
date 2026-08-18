using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class TeachingSupportHoursDataStore : ITeachingSupportHoursDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public TeachingSupportHoursDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid activityId, Guid academicLoadId, CancellationToken cancellationToken)
    {
        return _dbContext.TeachingSupportHours.AnyAsync(hours => hours.ActivityId == activityId && hours.AcademicLoadId == academicLoadId, cancellationToken);
    }

    public async Task AddTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursCreatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.TeachingSupportHours.AddAsync(teachingSupportHours, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Domain.Entities.TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.TeachingSupportHours.FirstOrDefaultAsync(hours => hours.TenantId == tenantId && hours.Id == id, cancellationToken);
    }

    public async Task UpdateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.TeachingSupportHours.Update(teachingSupportHours);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.TeachingSupportHours.Update(teachingSupportHours);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, TeachingSupportHoursActivatedIntegrationEvent integrationEvent,CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.TeachingSupportHours.Update(teachingSupportHours);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
          
    }
}