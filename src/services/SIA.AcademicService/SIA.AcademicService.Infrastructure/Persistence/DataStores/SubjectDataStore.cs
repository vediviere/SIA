using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class SubjectDataStore : ISubjectDataStore
{
  private readonly AcademicDbContext _dbContext;

  public SubjectDataStore(AcademicDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> SubjectCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    return _dbContext.Subjects.AnyAsync(
        subject => subject.TenantId == tenantId && subject.Code == code,
        cancellationToken);
  }

  public Task<Subject?> GetSubjectByIdAsync(Guid tenantId, Guid subjectId, CancellationToken cancellationToken)
  {
    return _dbContext.Subjects.FirstOrDefaultAsync(subject => subject.TenantId == tenantId && subject.Id == subjectId, cancellationToken);
  }

  public async Task AddSubjectWithOutboxAsync(Subject subject, SubjectCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.SubjectCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.Subjects.AddAsync(subject, cancellationToken);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task UpdateSubjectWithOutboxAsync(Subject subject, SubjectUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.SubjectUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.Subjects.Update(subject);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task SoftDeleteSubjectWithOutboxAsync(Subject subject, SubjectDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.SubjectDeletedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.Subjects.Update(subject);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task RestoreSubjectWithOutboxAsync(Subject subject, SubjectRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.SubjectRestoredV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    _dbContext.Subjects.Update(subject);
    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}



