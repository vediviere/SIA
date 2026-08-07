using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class DuplicateStudyPlanCodeException : ConflictException
{
    public DuplicateStudyPlanCodeException(string code)
        : base($"Ya existe un plan de estudios con el código {code}.")
    {
    }
}