using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;

public sealed class DuplicateClassroomTypeNameException : InvalidOperationException
{
    public DuplicateClassroomTypeNameException(string name)
        : base($"Ya existe un tipo de aula o laboratorio con el nombre '{name}' para esta institución.")
    {
    }
}
