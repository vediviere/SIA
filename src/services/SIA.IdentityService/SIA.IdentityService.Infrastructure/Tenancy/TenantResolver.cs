using System.Net;
using System.Net.Http.Json;
using SIA.IdentityService.Application.Common.Exceptions;
using SIA.IdentityService.Application.Interfaces.Tenancy;
using SIA.TenancyService.Contracts.Requests.Tenants;
using SIA.TenancyService.Contracts.Responses.Tenants;

namespace SIA.IdentityService.Infrastructure.Tenancy;

public sealed class TenantResolver : ITenantResolver
{
  private readonly HttpClient _httpClient;

  public TenantResolver(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Guid?> ResolveTenantIdAsync(string instituteCode, string email, CancellationToken cancellationToken)
  {
    var request = new ResolveTenantRequest
    {
      InstituteCode = instituteCode,
      Email = email
    };

    var response = await _httpClient.PostAsJsonAsync("api/tenants/resolve", request, cancellationToken);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return null;
    }

    if (response.StatusCode == HttpStatusCode.Conflict)
    {
      throw new InactiveTenantException(instituteCode);
    }

    if (response.StatusCode == HttpStatusCode.BadRequest)
    {
      throw new InvalidTenantEmailException(instituteCode);
    }

    response.EnsureSuccessStatusCode();

    var tenant = await response.Content.ReadFromJsonAsync<ResolveTenantResponse>(cancellationToken: cancellationToken);

    if (tenant is null || tenant.TenantId == Guid.Empty)
    {
      throw new InvalidOperationException("TenancyService devolvió una respuesta inválida.");
    }

    return tenant.TenantId;
  }
}
