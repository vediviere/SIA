using System;

namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency
{
    public sealed class TeacherCompetencyActivatedIntegrationEvent
    {
        public Guid EventId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public Guid TenantId { get; set; }
        public Guid TeacherCompetencyId { get; set; }
        public int Version { get; set; }
    }
}