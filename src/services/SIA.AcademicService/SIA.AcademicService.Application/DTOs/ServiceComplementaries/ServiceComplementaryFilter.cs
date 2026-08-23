using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.ServiceComplementaries;

public sealed class ServiceComplementaryFilter
{
    public Guid TenantId { get; init; }

    public Guid? StudyPlanId { get; init; }

    public bool? Type { get; init; }

    public bool? Status { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}