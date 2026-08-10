using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class ClassroomNotFoundException : NotFoundException
{
    public ClassroomNotFoundException(Guid classroomId)
        : base($"No se encontró el aula o laboratorio con el ID '{classroomId}'.")
    {
    }
}
