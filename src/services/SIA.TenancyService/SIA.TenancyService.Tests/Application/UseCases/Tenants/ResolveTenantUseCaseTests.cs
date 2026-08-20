using SIA.TenancyService.Application.Common.Exceptions;
using SIA.TenancyService.Application.Interfaces.Queries;
using SIA.TenancyService.Application.UseCases.Tenants;
using SIA.TenancyService.Contracts.Requests.Tenants;
using SIA.TenancyService.Domain.Entities;

namespace SIA.TenancyService.Tests.Application.UseCases.Tenants;

public sealed class ResolveTenantUseCaseTests
{
  [Fact]
  public async Task Resolve_WithValidData_ReturnsTenant()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");
    var useCase = CreateUseCase(tenant);

    var response = await useCase.ExecuteAsync(new ResolveTenantRequest
    {
      InstituteCode = " test001 ",
      Email = " Alumno@Institucion.edu.mx "
    }, CancellationToken.None);

    Assert.Equal(tenant.Id, response.TenantId);
    Assert.Equal("TEST001", response.InstituteCode);
  }

  [Fact]
  public async Task Resolve_WithUnknownCode_Throws()
  {
    var useCase = CreateUseCase(null);

    await Assert.ThrowsAsync<TenantNotFoundException>(() => useCase.ExecuteAsync(new ResolveTenantRequest
    {
      InstituteCode = "NO-EXISTE",
      Email = "alumno@institucion.edu.mx"
    }, CancellationToken.None));
  }

  [Fact]
  public async Task Resolve_WithInactiveTenant_Throws()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");
    tenant.Deactivate();

    var useCase = CreateUseCase(tenant);

    await Assert.ThrowsAsync<InactiveTenantException>(() => useCase.ExecuteAsync(new ResolveTenantRequest
    {
      InstituteCode = "TEST001",
      Email = "alumno@institucion.edu.mx"
    }, CancellationToken.None));
  }

  [Fact]
  public async Task Resolve_WithOtherDomain_Throws()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");
    var useCase = CreateUseCase(tenant);

    await Assert.ThrowsAsync<InvalidTenantEmailException>(() => useCase.ExecuteAsync(new ResolveTenantRequest
    {
      InstituteCode = "TEST001",
      Email = "alumno@otro-dominio.edu.mx"
    }, CancellationToken.None));
  }

  private static ResolveTenantUseCase CreateUseCase(Tenant? tenant)
  {
    return new ResolveTenantUseCase(new TenantQueriesStub(tenant));
  }

  private sealed class TenantQueriesStub : ITenantQueries
  {
    private readonly Tenant? _tenant;

    public TenantQueriesStub(Tenant? tenant)
    {
      _tenant = tenant;
    }

    public Task<Tenant?> GetByCodeAsync(string instituteCode, CancellationToken cancellationToken)
    {
      if (_tenant is null || !string.Equals(_tenant.InstituteCode, instituteCode, StringComparison.OrdinalIgnoreCase))
      {
        return Task.FromResult<Tenant?>(null);
      }

      return Task.FromResult<Tenant?>(_tenant);
    }
  }
}
