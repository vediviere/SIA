using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;

public sealed class DuplicateTeachingSupportHoursException : ConflictException
{
    public DuplicateTeachingSupportHoursException(Guid activityId, Guid academicLoadId) : base($"La actividad {activityId} ya está registrada para la carga académica {academicLoadId}.")
    {
    }
}
