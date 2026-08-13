using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class DuplicateCoordinatorException : ConflictException
{
    public DuplicateCoordinatorException(Guid personId)
        : base($"La persona con Id {personId} ya está registrada como coordinador.")
    {
    }
}