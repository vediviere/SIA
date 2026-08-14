using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class EducationalProgramsDataStore : IEducationalProgramDataStore
{
  private readonly AcademicDbContext _dbContext;

  public EducationalProgramsDataStore(AcademicDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> EducationalProgramCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    return _dbContext.EducationalPrograms.AnyAsync(educationalPrograms => educationalPrograms.TenantId == tenantId && educationalPrograms.Code == code, cancellationToken);
  }

  public async Task AddEducationalProgramWithOutboxAsync(EducationalProgram educationalPrograms, EducationalProgramCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.EducationalProgramCreatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<EducationalProgram?> GetByIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
  {
    return await _dbContext.EducationalPrograms.FirstOrDefaultAsync(
        educationalProgram => educationalProgram.TenantId == tenantId && educationalProgram.Id == educationalProgramId,
        cancellationToken);
  }

  public async Task UpdateEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.EducationalProgramUpdatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeactivateEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.EducationalProgramDeactivatedV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task RestoreEducationalProgramWithOutboxAsync(EducationalProgram educationalProgram, EducationalProgramRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var eventType = AcademicIntegrationEventTypes.EducationalProgramRestoredV1;
    var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
