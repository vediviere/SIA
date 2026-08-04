using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;


namespace SIA.AcademicService.Infrastructure.Persistence.Queries
{
    public class AcademicPeriodsQueries : IAcademicPeriodsQueries
    {

        private readonly AcademicDbContext _dbcontext;

        public AcademicPeriodsQueries(AcademicDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _dbcontext.AcademicPeriods.AsNoTracking().FirstOrDefaultAsync(academicPeriod => academicPeriod.Id == id, cancellationToken);
        }

        public Task<List<AcademicPeriod>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _dbcontext.AcademicPeriods.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
