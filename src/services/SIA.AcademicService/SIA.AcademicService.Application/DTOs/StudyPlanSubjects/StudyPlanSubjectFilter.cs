using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.StudyPlanSubjects;

public sealed class StudyPlanSubjectFilter
{
    public Guid TenantId { get; init; }

    public Guid? StudyPlanId { get; init; }

    public Guid? SubjectId { get; init; }

    public bool? IsRequired { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}