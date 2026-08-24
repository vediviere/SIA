namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class StudyPlanSubjectNotFoundException : Exception
{
    public StudyPlanSubjectNotFoundException(Guid id)
        : base($"No se encontró la asignación de materia en el plan de estudios con el ID {id}.")
    {
    }
}