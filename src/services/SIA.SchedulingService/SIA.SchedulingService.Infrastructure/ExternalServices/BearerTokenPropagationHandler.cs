using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace SIA.SchedulingService.Infrastructure.ExternalServices;

public sealed class BearerTokenPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenPropagationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var incomingAuthHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(incomingAuthHeader) && incomingAuthHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = incomingAuthHeader["Bearer ".Length..];
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}