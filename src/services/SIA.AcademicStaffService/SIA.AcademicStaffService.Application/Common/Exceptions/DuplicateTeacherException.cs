using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class DuplicateTeacherException : ConflictException
{
    public DuplicateTeacherException(Guid personId)
        : base($"La persona con Id {personId} ya está registrada como profesor.")
    {
    }
}