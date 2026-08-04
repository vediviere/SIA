using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface IAcademicPeriodQueries
    {
        Task<AcademicPeriod?> GetByIdAsync(Guid tenantId,Guid academicPeriodId,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<AcademicPeriod>> SearchAsync(AcademicPeriodFilter filter,CancellationToken cancellationToken);
    }
}
