namespace SIA.AdminBff.Clients.Workflow;

public interface IWorkflowClient
{
    Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken);
    Task ApproveReviewAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken);
    Task ReturnReviewAsync(Guid tenantId, Guid processId, ReturnRequestDto request, CancellationToken cancellationToken);
}