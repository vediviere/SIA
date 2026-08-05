namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class AcademicPeriodNotFoundException : NotFoundException
{
  public AcademicPeriodNotFoundException(Guid academicPeriodId)
      : base($"No se encontró el periodo académico con Id {academicPeriodId}.")
  {
  }
}
