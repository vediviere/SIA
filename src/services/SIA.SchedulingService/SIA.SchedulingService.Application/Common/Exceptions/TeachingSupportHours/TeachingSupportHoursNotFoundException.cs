using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;

public sealed class TeachingSupportHoursNotFoundException : NotFoundException
{
    public TeachingSupportHoursNotFoundException(Guid id) : base($"No se encontraron las horas de apoyo docente con Id {id}.")
    {
    }
}
