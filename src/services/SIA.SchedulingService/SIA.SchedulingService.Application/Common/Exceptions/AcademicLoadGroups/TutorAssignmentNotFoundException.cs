using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;

public sealed class TutorAssignmentNotFoundException : NotFoundException
{
    public TutorAssignmentNotFoundException(Guid groupId)
        : base($"No se encontró una asignación de tutoría vigente para el grupo con ID '{groupId}'.")
    {
    }
}
