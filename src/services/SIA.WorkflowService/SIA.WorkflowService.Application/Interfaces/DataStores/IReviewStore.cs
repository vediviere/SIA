using SIA.WorkflowService.Domain.Entities;

namespace SIA.WorkflowService.Application.Interfaces.DataStores;

public interface IReviewStore
{
  Task<bool> WasProcessedAsync(Guid eventId, CancellationToken cancellationToken);
  Task CreateAsync(ReviewProcess process, Guid eventId, string eventType, string sourceService, CancellationToken cancellationToken);
}
