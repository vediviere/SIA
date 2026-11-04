using System;

namespace SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency
{
    public sealed class CreateTeacherCompetencyRequest
    {
        public Guid CompetencyId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
    }
}