using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.DTOs.Persons;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Domain.Entities;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Queries;

public sealed class PersonQueries : IPersonQueries
{
    private readonly AcademicStaffDbContext _dbContext;

    public PersonQueries(AcademicStaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Person?> GetByIdAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
    {
        return await _dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == personId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Person>> SearchAsync(PersonFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Person> query = _dbContext.Persons
            .AsNoTracking()
            .Where(x => x.TenantId == filter.TenantId);

        if (!string.IsNullOrWhiteSpace(filter.EmployeeNumber))
        {
            query = query.Where(x => x.EmployeeNumber.Contains(filter.EmployeeNumber));
        }

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
        {
            query = query.Where(x => x.FirstName.Contains(filter.FirstName));
        }

        if (!string.IsNullOrWhiteSpace(filter.PaternalLastName))
        {
            query = query.Where(x => x.PaternalLastName.Contains(filter.PaternalLastName));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        query = query.Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.OrderBy(x => x.PaternalLastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);
    }
}