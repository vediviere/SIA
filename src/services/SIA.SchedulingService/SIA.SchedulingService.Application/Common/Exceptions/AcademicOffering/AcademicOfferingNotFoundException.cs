using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;

public sealed class AcademicOfferingNotFoundException : NotFoundException
{
    public AcademicOfferingNotFoundException(Guid id): base($"No se encontró la oferta académica con ID '{id}'.")
    {
    }
}