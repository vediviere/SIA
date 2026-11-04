using System;

namespace SIA.AcademicStaffService.Application.Common.Exceptions
{
    public sealed class TeacherCompetencyNotFoundException : Exception
    {
        public TeacherCompetencyNotFoundException(Guid teacherCompetencyId)
            : base($"No se encontró la competencia del docente con ID '{teacherCompetencyId}'.")
        {
        }
    }
}