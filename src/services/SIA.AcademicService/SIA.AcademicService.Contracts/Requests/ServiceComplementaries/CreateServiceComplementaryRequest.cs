using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Requests.ServiceComplementaries;

public sealed record CreateServiceComplementaryRequest
{
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required bool Type { get; init; }
    public required int Credit { get; init; }
}