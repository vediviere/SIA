using System;

namespace SIA.AcademicStaffService.Contracts.Responses.TeacherCompetency
{
    public sealed class UpdateTeacherCompetencyResponse
    {
        public Guid TeacherCompetencyId { get; set; }
        public Guid TenantId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid CompetencyId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public Guid CorrelationId { get; set; }
    }
}