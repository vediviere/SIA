using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions.SupportSchedules;

public sealed class SupportScheduleNotFoundException : NotFoundException
{
    public SupportScheduleNotFoundException(Guid supportScheduleId)
        : base($"No se encontró el horario de apoyo con el ID '{supportScheduleId}'.")
    {
    }
}
