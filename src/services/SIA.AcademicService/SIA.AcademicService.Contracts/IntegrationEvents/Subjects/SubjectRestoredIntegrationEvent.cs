using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.IntegrationEvents.Subjects;

public sealed class SubjectRestoredIntegrationEvent
{
    public required Guid EventId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required DateTime OccurredAtUtc { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid SubjectId { get; init; }
    public int Version { get; init; } = 1;
}