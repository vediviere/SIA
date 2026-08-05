using SIA.AcademicService.Application.DTOs.EducationalProgram;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public  interface IEducationalProgramQueries
    {
        Task<EducationalProgram?> GetByIdAsync(Guid tenantId,Guid educationalProgramId,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<EducationalProgram>>SearchAsync(EducationalProgramFilter filter,CancellationToken cancellationToken);
    }
}
