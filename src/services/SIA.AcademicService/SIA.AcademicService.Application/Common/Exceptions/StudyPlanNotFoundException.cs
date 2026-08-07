using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class StudyPlanNotFoundException : NotFoundException
{
    public StudyPlanNotFoundException(Guid studyPlanId)
        : base($"No se encontró un plan de estudios con el id {studyPlanId}.")
    {
    }
}