using SIA.BuildingBlocks.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadGroups;

public sealed class GroupAlreadyHasTutorException : ConflictException
{
    public GroupAlreadyHasTutorException(Guid groupId)
        : base($"El grupo con ID '{groupId}' ya tiene un tutor académico vigente asignado.")
    {
    }
}
