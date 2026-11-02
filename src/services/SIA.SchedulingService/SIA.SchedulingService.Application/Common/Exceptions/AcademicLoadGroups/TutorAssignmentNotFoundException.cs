using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;

public sealed class TutorAssignmentNotFoundException : NotFoundException
{
    public TutorAssignmentNotFoundException(Guid groupId)
        : base($"No se encontró una asignación de tutoría vigente para el grupo con ID '{groupId}'.")
    {
    }
}
