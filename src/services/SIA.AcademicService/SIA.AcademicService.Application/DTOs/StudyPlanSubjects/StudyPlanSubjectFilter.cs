using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.StudyPlanSubjects;

public sealed class StudyPlanSubjectFilter
{
    public Guid TenantId { get; set; }

    public Guid? StudyPlanId { get; set; }

    public Guid? SubjectId { get; set; }

    public bool? IsRequired { get; set; }

    public bool? Status { get; set; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}