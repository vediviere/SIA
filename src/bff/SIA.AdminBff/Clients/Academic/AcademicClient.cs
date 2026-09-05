using System.Net;
using System.Net.Http.Json;
using SIA.AdminBff.Configuration;
using SIA.AdminBff.Infrastructure.Errors;

namespace SIA.AdminBff.Clients.Academic;

public sealed class AcademicClient : IAcademicClient
{
  private readonly HttpClient _httpClient;

  public AcademicClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<AcademicContextDto> GetAcademicContextAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, $"api/academic-context/educational-programs/{educationalProgramId}");
    request.Headers.TryAddWithoutValidation("tenantid", tenantId.ToString());

    using var response = await _httpClient.SendAsync(request, cancellationToken);
    response.EnsureInternalSuccess(InternalServiceConfiguration.AcademicService);

    return await response.Content.ReadFromJsonAsync<AcademicContextDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.AcademicService, HttpStatusCode.BadGateway);
  }
}
