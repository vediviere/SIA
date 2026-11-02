using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;

public sealed class GroupAlreadyHasTutorException : ConflictException
{
    public GroupAlreadyHasTutorException(Guid groupId)
        : base($"El grupo con ID '{groupId}' ya tiene un tutor académico vigente asignado.")
    {
    }
}
