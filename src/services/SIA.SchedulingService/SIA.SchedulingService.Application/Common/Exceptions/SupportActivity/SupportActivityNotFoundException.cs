using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions.SupportActivity;

public sealed class SupportActivityNotFoundException : NotFoundException
{
    public SupportActivityNotFoundException(Guid supportActivityId)
        : base($"No se encontró la actividad de apoyo con el ID '{supportActivityId}'.")
    {
    }
}