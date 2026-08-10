using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class DuplicateClassroomCodeException : InvalidOperationException
{
    public DuplicateClassroomCodeException(string code)
        : base($"Ya existe un aula o laboratorio con el código '{code}' para esta institución.")
    {
    }
}
