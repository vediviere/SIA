namespace SIA.AdminBff.Clients.Workflow;

public interface IWorkflowClient
{
    Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(CancellationToken cancellationToken);
    Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid processId, CancellationToken cancellationToken);
    Task ApproveReviewAsync(Guid processId, CancellationToken cancellationToken);
    Task ReturnReviewAsync(Guid processId, ReturnRequestDto request, CancellationToken cancellationToken);
}