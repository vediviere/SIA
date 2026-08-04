using SIA.AcademicService.Application.DTOs.EducationalProgram;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public  interface IEducationalProgramQueries
    {
        Task<EducationalPrograms?> GetByIdAsync(Guid tenantId,Guid educationalProgramId,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<EducationalPrograms>>SearchAsync(EducationalProgramFilter filter,CancellationToken cancellationToken);
    }
}
