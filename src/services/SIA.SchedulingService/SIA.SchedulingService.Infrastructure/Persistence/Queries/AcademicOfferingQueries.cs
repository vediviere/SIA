using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;

namespace SIA.SchedulingService.Infrastructure.Persistence.Queries;

public sealed class AcademicOfferingQueries : IAcademicOfferingQueries
{
    private readonly SchedulingDbContext _dbContext;

    public AcademicOfferingQueries(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AcademicOffering?> GetByIdAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken)
    {
        return _dbContext.AcademicOfferings.AsNoTracking().FirstOrDefaultAsync(offering => offering.TenantId == tenantId && offering.Id == offeringId, cancellationToken);
    }
}