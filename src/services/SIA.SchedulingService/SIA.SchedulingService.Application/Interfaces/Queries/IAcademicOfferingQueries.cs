using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.Queries;

public interface IAcademicOfferingQueries
{
    Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken);
}