using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class AcademicLoadNotFoundException : NotFoundException
{
    public AcademicLoadNotFoundException(Guid academicLoadId) : base($"No se encontró la carga académica con Id {academicLoadId}.")
    {
    }
}