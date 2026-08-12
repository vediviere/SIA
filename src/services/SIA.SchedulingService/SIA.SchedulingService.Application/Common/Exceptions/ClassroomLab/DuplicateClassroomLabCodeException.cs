using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;

public sealed class DuplicateClassroomLabCodeException : InvalidOperationException
{
    public DuplicateClassroomLabCodeException(string code)
        : base($"Ya existe un aula o laboratorio con el código '{code}' para esta institución.")
    {
    }
}
