using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class GroupNotFoundException :NotFoundException
{
    public GroupNotFoundException(Guid GroupId) : base($"No se encontro el grupo con el Id {GroupId}.")
    {
    }
}