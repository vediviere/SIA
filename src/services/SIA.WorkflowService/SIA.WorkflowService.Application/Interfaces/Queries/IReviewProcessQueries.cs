using SIA.WorkflowService.Contracts.Responses.ReviewProcesses;

namespace SIA.WorkflowService.Application.Interfaces.Queries;

public interface IReviewProcessQueries
{
    Task<IReadOnlyList<ReviewProcessListItemResponse>> GetInReviewAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task<ReviewProcessResponse?> GetByIdAsync(
        Guid tenantId,
        Guid processId,
        CancellationToken cancellationToken);
}