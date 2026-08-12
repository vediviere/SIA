using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;

public sealed class ClassroomLabNotFoundException : NotFoundException
{
    public ClassroomLabNotFoundException(Guid classroomId)
        : base($"No se encontró el aula o laboratorio con el ID '{classroomId}'.")
    {
    }
}
