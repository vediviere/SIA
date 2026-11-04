using System;

namespace SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency
{
    public sealed class TeacherCompetencyUpdatedIntegrationEvent
    {
        public Guid EventId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public Guid TenantId { get; set; }
        public Guid TeacherCompetencyId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid CompetencyId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int Version { get; set; }
    }
}