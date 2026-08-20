using SIA.TenancyService.Application.Common.Exceptions;
using SIA.TenancyService.Application.Interfaces.Queries;
using SIA.TenancyService.Contracts.Requests.Tenants;
using SIA.TenancyService.Contracts.Responses.Tenants;

namespace SIA.TenancyService.Application.UseCases.Tenants;

public sealed class ResolveTenantUseCase
{
  private readonly ITenantQueries _tenantQueries;

  public ResolveTenantUseCase(ITenantQueries tenantQueries)
  {
    _tenantQueries = tenantQueries;
  }

  public async Task<ResolveTenantResponse> ExecuteAsync(ResolveTenantRequest request, CancellationToken cancellationToken)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    if (string.IsNullOrWhiteSpace(request.InstituteCode))
    {
      throw new ArgumentException("El código institucional es obligatorio.", nameof(request.InstituteCode));
    }

    if (string.IsNullOrWhiteSpace(request.Email))
    {
      throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));
    }

    var instituteCode = request.InstituteCode.Trim().ToUpperInvariant();
    var email = request.Email.Trim().ToLowerInvariant();

    var tenant = await _tenantQueries.GetByCodeAsync(instituteCode, cancellationToken);

    if (tenant is null)
    {
      throw new TenantNotFoundException(instituteCode);
    }

    if (!tenant.IsActive)
    {
      throw new InactiveTenantException(instituteCode);
    }

    if (!tenant.AllowsEmail(email))
    {
      throw new InvalidTenantEmailException(instituteCode);
    }

    return new ResolveTenantResponse
    {
      TenantId = tenant.Id,
      InstituteCode = tenant.InstituteCode
    };
  }
}
