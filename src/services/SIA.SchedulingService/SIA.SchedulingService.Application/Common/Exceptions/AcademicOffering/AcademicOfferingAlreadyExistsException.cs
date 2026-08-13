using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;

public sealed class AcademicOfferingAlreadyExistsException : ConflictException
{
    public AcademicOfferingAlreadyExistsException(Guid groupId, Guid subjectId): base($"La materia con ID '{subjectId}' ya se encuentra ofertada para el grupo con ID '{groupId}'.")
    {
    }
}