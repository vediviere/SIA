using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.Entities;
using System.Text.Json;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.WorkflowService.Infrastructure.Persistence.DataStores;

public sealed class ReviewStore : IReviewStore
{
  private readonly WorkflowDbContext _dbContext;

  public ReviewStore(WorkflowDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<bool> WasProcessedAsync(Guid eventId, CancellationToken cancellationToken)
  {
    return _dbContext.InboxMessages.AsNoTracking().AnyAsync(message => message.Id == eventId, cancellationToken);
  }

  public Task<ReviewProcess?> GetByIdAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
  {
    return _dbContext.ReviewProcesses
      .Include(process => process.Observations)
      .FirstOrDefaultAsync(process => process.TenantId == tenantId && process.Id == processId, cancellationToken);
  }

  public async Task CreateAsync(ReviewProcess process, Guid eventId, string eventType, string sourceService, CancellationToken cancellationToken)
  {
    var inboxMessage = new InboxMessage(eventId, eventType, sourceService, process.CorrelationId);

    await _dbContext.ReviewProcesses.AddAsync(process, cancellationToken);
    await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);

    inboxMessage.MarkAsProcessed();

    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task SaveDecisionAsync<TEvent>(ReviewProcess process, TEvent integrationEvent, string eventType, Guid correlationId, CancellationToken cancellationToken) where TEvent : class
  {
    var payload = JsonSerializer.Serialize(integrationEvent);
    var outboxMessage = new OutboxMessage(eventType, payload, correlationId);

    foreach (var observation in process.Observations)
    {
      if (_dbContext.Entry(observation).State == EntityState.Detached)
      {
        await _dbContext.ReviewObservations.AddAsync(observation, cancellationToken);
      }
    }

    await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
