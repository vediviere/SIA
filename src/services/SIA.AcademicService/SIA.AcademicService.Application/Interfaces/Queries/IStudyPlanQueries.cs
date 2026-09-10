
﻿using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface IStudyPlanQueries

    {
        Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<StudyPlan>> SearchAsync(StudyPlanFilter filter, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<StudyPlanSubjectDto>> GetSubjectsByStudyPlanAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken);

        Task<StudyPlan?> GetActiveByProgramIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken);
    }
}
