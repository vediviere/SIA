using SIA.TenancyService.Domain.Entities;

namespace SIA.TenancyService.Tests.Domain;

public sealed class TenantTests
{
  [Fact]
  public void Constructor_NormalizesValues()
  {
    var tenant = new Tenant(" test001 ", " Institución de prueba ", " @Institucion.EDU.MX ");

    Assert.NotEqual(Guid.Empty, tenant.Id);
    Assert.Equal("TEST001", tenant.InstituteCode);
    Assert.Equal("Institución de prueba", tenant.Name);
    Assert.Equal("institucion.edu.mx", tenant.EmailDomain);
    Assert.True(tenant.IsActive);
    Assert.NotEqual(default, tenant.CreatedAtUtc);
    Assert.Null(tenant.UpdatedAtUtc);
  }

  [Fact]
  public void AllowsEmail_WithMatchingDomain_ReturnsTrue()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");

    var result = tenant.AllowsEmail(" Alumno@Institucion.edu.mx ");

    Assert.True(result);
  }

  [Fact]
  public void AllowsEmail_WithOtherDomain_ReturnsFalse()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");

    var result = tenant.AllowsEmail("alumno@otro-dominio.edu.mx");

    Assert.False(result);
  }

  [Fact]
  public void Deactivate_SetsInactive()
  {
    var tenant = new Tenant("TEST001", "Institución de prueba", "institucion.edu.mx");

    tenant.Deactivate();

    Assert.False(tenant.IsActive);
    Assert.NotNull(tenant.UpdatedAtUtc);
  }

  [Fact]
  public void Constructor_WithoutCode_Throws()
  {
    Assert.Throws<ArgumentException>(() => new Tenant("", "Institución de prueba", "institucion.edu.mx"));
  }
}
