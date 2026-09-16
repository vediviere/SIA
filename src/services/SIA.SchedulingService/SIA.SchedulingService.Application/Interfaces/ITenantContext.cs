namespace SIA.SchedulingService.Application.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
}