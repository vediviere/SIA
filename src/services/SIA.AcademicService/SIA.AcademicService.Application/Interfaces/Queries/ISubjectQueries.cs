using SIA.AcademicService.Application.DTOs.Subjects;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface ISubjectQueries
    {
        Task<Subject?> GetByIdAsync(Guid tenantId,Guid subjectId,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Subject>> SearchAsync(SubjectFilter filter, CancellationToken cancellationToken);
    }
}
