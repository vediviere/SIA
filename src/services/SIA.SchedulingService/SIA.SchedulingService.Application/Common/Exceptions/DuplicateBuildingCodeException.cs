using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class DuplicateBuildingCodeException : ConflictException
{
    public DuplicateBuildingCodeException(string code) : base($"Ya existe un edificio con el código {code} para esta institución.")
    {

    }
}