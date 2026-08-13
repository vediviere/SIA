using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class CoordinatorNotFoundException : NotFoundException
{
    public CoordinatorNotFoundException(Guid coordinatorId)
        : base($"No se encontró el coordinador con Id {coordinatorId}.")
    {
    }
}