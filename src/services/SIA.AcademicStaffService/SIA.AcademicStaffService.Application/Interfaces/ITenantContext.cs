namespace SIA.AcademicStaffService.Application.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
}