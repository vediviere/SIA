using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;

public sealed class ClassroomTypeNotFoundException : NotFoundException
{
    public ClassroomTypeNotFoundException(Guid classroomTypeId)
        : base($"No se encontró el tipo de aula con el ID '{classroomTypeId}'.")
    {
    }

}
