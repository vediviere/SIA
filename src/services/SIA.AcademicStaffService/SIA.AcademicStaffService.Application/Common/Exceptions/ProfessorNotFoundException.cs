using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class ProfessorNotFoundException : NotFoundException
{
    public ProfessorNotFoundException(Guid professorId)
        : base($"No se encontró el profesor con Id {professorId}.")
    {
    }
}