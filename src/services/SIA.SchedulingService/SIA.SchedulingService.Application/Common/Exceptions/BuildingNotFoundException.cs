
using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class BuildingNotFoundException : NotFoundException
{
    public BuildingNotFoundException(Guid buildingId) : base($"No se encontró el edificio con Id {buildingId}.")
    {
    }
}