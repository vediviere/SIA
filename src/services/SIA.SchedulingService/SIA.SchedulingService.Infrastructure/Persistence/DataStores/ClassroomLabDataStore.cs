using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using System.Text.Json;


namespace SIA.SchedulingService.Infrastructure.Persistence.DataStores;

public sealed class ClassroomLabDataStore : IClassroomLabDataStore
{
    private readonly SchedulingDbContext _dbContext;

    public ClassroomLabDataStore(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ClassroomLabCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomLabs.AnyAsync(
            classroomLab => classroomLab.TenantId == tenantId && classroomLab.Code == code,
            cancellationToken);
    }

    public Task<ClassroomLab?> GetClassroomLabByIdAsync(Guid tenantId, Guid classroomLabId, CancellationToken cancellationToken)
    {
        return _dbContext.ClassroomLabs
            .Include(classroomLab => classroomLab.ClassroomType)
            .FirstOrDefaultAsync(classroomLab => classroomLab.TenantId == tenantId && classroomLab.Id == classroomLabId, cancellationToken);
    }

    public async Task AddClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassroomLabCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.ClassroomLabs.AddAsync(classroomLab, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassroomLabUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassroomLabs.Update(classroomLab);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassroomLabDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassroomLabs.Update(classroomLab);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreClassroomLabWithOutboxAsync(ClassroomLab classroomLab, ClassroomLabRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = SchedulingIntegrationEventTypes.ClassroomLabRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ClassroomLabs.Update(classroomLab);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}