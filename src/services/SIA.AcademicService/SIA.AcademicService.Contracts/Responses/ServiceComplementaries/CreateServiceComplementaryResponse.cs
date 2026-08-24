using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Responses.ServiceComplementaries;

public sealed record CreateServiceComplementaryResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid StudyPlanId { get; init; }
    public required bool Type { get; init; }
    public required int Credit { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}