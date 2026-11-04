namespace SIA.AcademicStaffService.Contracts.Requests.TeacherCompetency
{
    public sealed class UpdateTeacherCompetencyRequest
    {
        public string Level { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
    }
}