using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.DTOs.Teacher;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Queries;

public sealed class TeacherQueries : ITeacherQueries
{
    private readonly AcademicStaffDbContext _dbContext;

    public TeacherQueries(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Teacher?> GetByIdAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken)
    {
        return await _dbContext.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == teacherId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Teacher>> SearchAsync(TeacherFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Teacher> query = _dbContext.Teachers
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (filter.PersonId.HasValue)
        {
            query = query.Where(x => x.PersonId == filter.PersonId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.ContractType))
        {
            query = query.Where(x => x.ContractType.Contains(filter.ContractType));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.OrderBy(x => x.ContractType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TeacherCandidateDto>> GetCandidatesAsync(CandidateTeacherFilter filter, CancellationToken cancellationToken)
    {
        return await _dbContext.Teachers
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId && x.Status)
            .Select(t => new TeacherCandidateDto
            {
                Id = t.Id,
                TenantId = t.TenantId,
                PersonId = t.PersonId,
                ProfessionalProfile = t.ProfessionalProfile,
                ContractType = t.ContractType,
                ContractHours = t.ContractHours,
                ProgramId = t.ProgramId,
                Status = t.Status,
                Competencies = t.TeacherCompetencies
                    .Where(tc => tc.Status)
                    .Select(tc => new TeacherCompetencyDto
                    {
                        TeacherCompetencyId = tc.TeacherCompetencyId,
                        CompetencyId = tc.CompetencyId,
                        Level = tc.Level,
                        Stage = tc.Stage,
                        Status = tc.Status
                    })
                    .ToList()
            })
            .OrderBy(x => x.ProgramId == null)
            .ThenBy(x => x.ProfessionalProfile)
            .ToListAsync(cancellationToken);
    }
}