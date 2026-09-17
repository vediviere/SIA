namespace SIA.AdminBff.Clients.Workflow;

public interface IWorkflowClient
{
    Task<IEnumerable<ReviewProcessDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<ReviewProcessDto> GetReviewByIdAsync(Guid tenantId, Guid reviewId, CancellationToken cancellationToken);
}