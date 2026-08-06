using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class DuplicateAcademicPeriodCodeException : ConflictException
{
  public DuplicateAcademicPeriodCodeException(string code)
      : base(
          $"Ya existe un periodo académico con el código {code} para esta institución.")
  {
  }
}
