using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;

public sealed record ServiceComplementaryDeletedIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid ServiceComplementaryId { get; init; }
    public int Version { get; init; } = 1;
}