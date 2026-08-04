using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.StudyPlan
{
    public sealed class StudyPlanSubjectDto
    {
        public Guid TenantId { get; init; }

        public Guid? StudyPlanId { get; init; }

        public Guid? SubjectId { get; init; }

        public string? Code { get; init; }

        public string? Name { get; init; }

        public int? Semester { get; init; }

        public int? Credits { get; init; }

        public bool? IsRequired { get; init; }

        public bool? Status { get; init; }
    }
}
