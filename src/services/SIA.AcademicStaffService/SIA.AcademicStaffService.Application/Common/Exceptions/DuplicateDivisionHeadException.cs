using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class DuplicateDivisionHeadException : ConflictException
{
    public DuplicateDivisionHeadException(Guid programId, Guid personId)
        : base($"La persona con Id {personId} ya es responsable del programa {programId}.")
    {
    }
}