using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class EducationalProgramNotFoundException : NotFoundException
{
    public EducationalProgramNotFoundException(Guid educationalProgramId)
        : base($"No se encontró el programa educativo con Id {educationalProgramId}.")
    {
    }
}
