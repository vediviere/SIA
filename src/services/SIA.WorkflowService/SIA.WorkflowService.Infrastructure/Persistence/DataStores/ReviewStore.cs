using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.Entities;

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

  public async Task CreateAsync(ReviewProcess process, Guid eventId, string eventType, string sourceService, CancellationToken cancellationToken)
  {
    var inboxMessage = new InboxMessage(eventId, eventType, sourceService, process.CorrelationId);

    await _dbContext.ReviewProcesses.AddAsync(process, cancellationToken);
    await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);

    inboxMessage.MarkAsProcessed();

    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
