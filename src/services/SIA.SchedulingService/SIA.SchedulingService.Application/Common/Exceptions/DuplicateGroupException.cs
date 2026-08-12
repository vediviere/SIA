using SIA.BuildingBlocks.Application.Exceptions;


namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class DuplicateGroupException : ConflictException
{
    public DuplicateGroupException(string GroupName, string shift) : base($"Ya existe un grupo con el nombre {GroupName} en el turno {shift} para este programa.")
    {
    }
}