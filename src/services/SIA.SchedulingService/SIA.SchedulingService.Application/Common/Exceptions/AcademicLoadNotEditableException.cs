using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.SchedulingService.Application.Common.Exceptions;

public sealed class AcademicLoadNotEditableException : ConflictException
{
  public AcademicLoadNotEditableException(Guid academicLoadId) : base($"La propuesta de carga académica con Id {academicLoadId} solo puede modificarse mientras permanezca activa y como borrador.")
  {
  }
}
