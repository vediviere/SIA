using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.AcademicService.Contracts.IntegrationEvents;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class AcademicPeriodsDataStore : IAcademicPeriodsDataStore
{
  private readonly AcademicDbContext _dbContext;

  public AcademicPeriodsDataStore(AcademicDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> AcademicPeriodCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicPeriods.AnyAsync(academicPeriod => academicPeriod.TenantId == tenantId && academicPeriod.Code == code, cancellationToken);
  }

  public async Task AddAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.AcademicPeriodCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.AcademicPeriods.AddAsync(academicPeriod, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

  }

  public Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicPeriods.FirstOrDefaultAsync(academicPeriods => academicPeriods.Id == id, cancellationToken);
  }

  public async Task UpdateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.AcademicPeriodUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeactivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.AcademicPeriodDeactivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task ActivateAcademicPeriodWithOutboxAsync(AcademicPeriod academicPeriod, AcademicPeriodActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.AcademicPeriodActivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

  }
}
