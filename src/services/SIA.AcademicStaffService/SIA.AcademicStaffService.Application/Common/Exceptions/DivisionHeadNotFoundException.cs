using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class DivisionHeadNotFoundException : NotFoundException
{
    public DivisionHeadNotFoundException(Guid divisionManagerId)
        : base($"No se encontró el responsable de división con Id {divisionManagerId}.")
    {
    }
}