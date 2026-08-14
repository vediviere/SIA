using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class StudyPlanDataStore : IStudyPlanDataStore
{
  private readonly AcademicDbContext _dbContext;

  public StudyPlanDataStore(AcademicDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> StudyPlanCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    return _dbContext.StudyPlans.AnyAsync(studyPlan => studyPlan.TenantId == tenantId && studyPlan.Code == code, cancellationToken);
  }

  public async Task AddStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.StudyPlanCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken)
  {
    return await _dbContext.StudyPlans.FirstOrDefaultAsync(
        studyPlan => studyPlan.TenantId == tenantId && studyPlan.Id == studyPlanId,
        cancellationToken);
  }

  public async Task UpdateStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.StudyPlanUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeactivateStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.StudyPlanDeactivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task RestoreStudyPlanWithOutboxAsync(StudyPlan studyPlan, StudyPlanRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.StudyPlanRestoredV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
