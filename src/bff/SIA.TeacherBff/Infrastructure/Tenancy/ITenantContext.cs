namespace SIA.TeacherBff.Infrastructure.Tenancy;

public interface ITenantContext
{
  Guid TenantId { get; }
}
