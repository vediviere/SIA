using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class PersonNotFoundException : NotFoundException
{
    public PersonNotFoundException(Guid personId) 
        : base($"No se encontro la persona con Id {personId}.")
    {
    }
}
