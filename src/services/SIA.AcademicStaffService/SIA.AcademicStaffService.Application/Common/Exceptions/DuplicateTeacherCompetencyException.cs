using System;

namespace SIA.AcademicStaffService.Application.Common.Exceptions
{
    public sealed class DuplicateTeacherCompetencyException : Exception
    {
        public DuplicateTeacherCompetencyException(Guid teacherId, Guid competencyId)
            : base($"El docente con ID '{teacherId}' ya tiene registrada la competencia con ID '{competencyId}'.")
        {
        }
    }
}