using SIA.AcademicService.Application.DTOs.ServiceComplementaries;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries

{
    public interface IServiceComplementaryQueries
    {
        Task<ServiceComplementary?> GetByIdAsync(Guid tenantId, Guid serviceComplementaryId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ServiceComplementary>> SearchAsync(ServiceComplementaryFilter filter, CancellationToken cancellationToken);
    }
}