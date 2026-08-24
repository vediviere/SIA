using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Responses.ServiceComplementaries;

public sealed record UpdateServiceComplementaryResponse
{
    public required Guid Id { get; init; }
    public required bool Type { get; init; }
    public required int Credit { get; init; }
    public required DateTime? UpdatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}