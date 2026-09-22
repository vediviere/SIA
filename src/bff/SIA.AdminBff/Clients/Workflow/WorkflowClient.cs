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

    public async Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/review-processes");
        request.Headers.TryAddWithoutValidation("tenantid", tenantId.ToString());

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureInternalSuccess(InternalServiceConfiguration.WorkflowService);

        return await response.Content.ReadFromJsonAsync<IEnumerable<ReviewProcessListItemDto>>(cancellationToken: cancellationToken)
            ?? Enumerable.Empty<ReviewProcessListItemDto>();
    }

    public async Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/review-processes/{processId}");
        request.Headers.TryAddWithoutValidation("tenantid", tenantId.ToString());

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureInternalSuccess(InternalServiceConfiguration.WorkflowService);

        return await response.Content.ReadFromJsonAsync<ReviewProcessDetailDto>(cancellationToken: cancellationToken);
    }

    public async Task ApproveReviewAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/review-processes/{processId}/approve");
        request.Headers.TryAddWithoutValidation("tenantid", tenantId.ToString());

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureInternalSuccess(InternalServiceConfiguration.WorkflowService);
    }

    public async Task ReturnReviewAsync(Guid tenantId, Guid processId, ReturnRequestDto requestDto, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/review-processes/{processId}/return")
        {
            Content = JsonContent.Create(requestDto)
        };
        request.Headers.TryAddWithoutValidation("tenantid", tenantId.ToString());

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureInternalSuccess(InternalServiceConfiguration.WorkflowService);
    }
}