using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class TeachingSupportHoursDataStore : ITeachingSupportHoursDataStore
{
  private readonly SchedulingDbContext _dbContext;

  public TeachingSupportHoursDataStore(SchedulingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> ExistsByActivityAndAcademicLoadAsync(Guid tenantId, Guid activityId, Guid academicLoadId, CancellationToken cancellationToken)
  {
    return _dbContext.TeachingSupportHours.AnyAsync(supportHour => supportHour.TenantId == tenantId && supportHour.ActivityId == activityId && supportHour.AcademicLoadId == academicLoadId, cancellationToken);
  }

  public async Task AddTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, AcademicLoad academicLoad, TeachingSupportHoursCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.TeachingSupportHours.AddAsync(teachingSupportHours, cancellationToken);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<Domain.Entities.TeachingSupportHour?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
  {
    return _dbContext.TeachingSupportHours.FirstOrDefaultAsync(hours => hours.TenantId == tenantId && hours.Id == id, cancellationToken);
  }

  public async Task UpdateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, AcademicLoad academicLoad, TeachingSupportHoursUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.TeachingSupportHours.Update(teachingSupportHours);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeactivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, AcademicLoad academicLoad, TeachingSupportHoursDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.TeachingSupportHours.Update(teachingSupportHours);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task ActivateTeachingSupportHoursWithOutboxAsync(Domain.Entities.TeachingSupportHour teachingSupportHours, AcademicLoad academicLoad, TeachingSupportHoursActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.TeachingSupportHours.Update(teachingSupportHours);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

  }
}
