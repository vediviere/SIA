using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;

public sealed class GroupNotEligibleForTutorException : ConflictException
{
    public GroupNotEligibleForTutorException(Guid groupId)
        : base($"El grupo con ID '{groupId}' no existe o no se encuentra aperturado.")
    {
    }
}
