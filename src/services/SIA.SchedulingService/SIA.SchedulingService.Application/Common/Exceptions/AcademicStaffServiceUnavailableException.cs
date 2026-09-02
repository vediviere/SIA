using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class AcademicStaffServiceUnavailableException : ConflictException
{
    public AcademicStaffServiceUnavailableException()
        : base("No fue posible obtener la información de docentes de AcademicStaffService.")
    {
    }
}