using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.BuildingBlocks.Messaging.Outbox;

using System.Text.Json;

namespace SIA.AcademicService.Infrastructure.Persistence.DataStores;

public sealed class ServiceComplementaryDataStore : IServiceComplementaryDataStore
{
    private readonly AcademicDbContext _dbContext;

    public ServiceComplementaryDataStore(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ServiceComplementary?> GetServiceComplementaryByIdAsync(Guid tenantId, Guid serviceComplementaryId, CancellationToken cancellationToken)
    {
        return _dbContext.ServiceComplementaries.FirstOrDefaultAsync(
            sc => sc.TenantId == tenantId && sc.Id == serviceComplementaryId,
            cancellationToken);
    }

    public async Task AddServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.ServiceComplementaryCreatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        await _dbContext.ServiceComplementaries.AddAsync(serviceComplementary, cancellationToken);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.ServiceComplementaryUpdatedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ServiceComplementaries.Update(serviceComplementary);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.ServiceComplementaryDeletedV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ServiceComplementaries.Update(serviceComplementary);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreServiceComplementaryWithOutboxAsync(ServiceComplementary serviceComplementary, ServiceComplementaryRestoredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(integrationEvent);
        var eventType = AcademicIntegrationEventTypes.ServiceComplementaryRestoredV1;
        var outboxMessage = new OutboxMessage(eventType, payload, integrationEvent.CorrelationId);

        _dbContext.ServiceComplementaries.Update(serviceComplementary);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}