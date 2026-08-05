
﻿using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Interfaces.Queries
{
    public interface IStudyPlanQueries

    {
        Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<StudyPlan>> SearchAsync(StudyPlanFilter filter, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<StudyPlanSubjectDto>> GetSubjectsByStudyPlanAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken);
    }
}
