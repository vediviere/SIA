using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class DuplicateEducationalProgramCodeException : ConflictException
{
    public DuplicateEducationalProgramCodeException(string code)
        : base($"Ya existe un programa educativo con el código {code} para esta institución.")
    {
    }
}
