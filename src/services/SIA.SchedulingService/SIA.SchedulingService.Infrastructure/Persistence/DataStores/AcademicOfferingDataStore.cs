using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using System.Text.Json;

namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class AcademicOfferingDataStore : IAcademicOfferingDataStore
{
  private readonly SchedulingDbContext _dbContext;

  public AcademicOfferingDataStore(SchedulingDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> ExistsByGroupAndSubjectAsync(Guid tenantId, Guid groupId, Guid subjectId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicOfferings.AnyAsync(offering =>
      offering.TenantId == tenantId &&
      offering.GroupId == groupId &&
      offering.SubjectId == subjectId,
      cancellationToken);
  }

  public async Task AddAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingCreatedIntegrationEvet integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.AcademicOfferingCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.AcademicOfferings.AddAsync(academicOffering, cancellationToken);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken)
  {
    return _dbContext.AcademicOfferings.FirstOrDefaultAsync(offering =>
      offering.TenantId == tenantId &&
      offering.Id == offeringId,
      cancellationToken);
  }

  public async Task UpdateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.AcademicOfferingStatusUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.AcademicOfferings.Update(academicOffering);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeactivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.AcademicOfferingDeactivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.AcademicOfferings.Update(academicOffering);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task ActivateAcademicOfferingWithOutboxAsync(AcademicOffering academicOffering, AcademicLoad academicLoad, AcademicOfferingActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = SchedulingIntegrationEventTypes.AcademicOfferingActivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.AcademicOfferings.Update(academicOffering);
    _dbContext.AcademicLoad.Update(academicLoad);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
