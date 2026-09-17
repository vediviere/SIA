using System.Net;
using System.Net.Http.Json;
using SIA.AdminBff.Configuration;
using SIA.AdminBff.Infrastructure.Errors;

namespace SIA.AdminBff.Clients.Workflow;

public sealed class WorkflowClient : IWorkflowClient
{
    private readonly HttpClient _httpClient;

    public WorkflowClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ReviewProcessDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ReviewProcessDto> GetReviewByIdAsync(Guid tenantId, Guid reviewId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}