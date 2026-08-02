using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.Queries;

public interface IEducationalProgramsQueries
{
    public EducationalPrograms GetById(Guid id);
    public List<EducationalPrograms> GetAll();
}
