using SIA.AcademicService.Domain.Entities;


namespace SIA.AcademicService.Application.Interfaces.Queries;

public interface IAcademicPeriodsQueries
{
    Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<AcademicPeriod>> GetAllAsync(CancellationToken cancellationToken);
}