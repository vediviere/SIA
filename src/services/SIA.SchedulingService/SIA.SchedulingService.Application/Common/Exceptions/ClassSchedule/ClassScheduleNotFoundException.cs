using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions.ClassSchedule;

public sealed class ClassScheduleNotFoundException : NotFoundException
{
    public ClassScheduleNotFoundException(Guid classScheduleId)
        : base($"No se encontró el horario de clase con el ID '{classScheduleId}'.")
    {
    }
}