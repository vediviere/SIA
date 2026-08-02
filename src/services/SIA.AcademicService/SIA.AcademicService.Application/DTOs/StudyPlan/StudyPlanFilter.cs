using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.StudyPlan
{
    public sealed class StudyPlanFilter
    {
        public Guid TenantId { get; init; }

        public Guid? EducationalProgramId { get; init; }

        public string? Code { get; init; }

        public string? Name { get; init; }

        public string? Version { get; init; }

        public bool? Status { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}
    