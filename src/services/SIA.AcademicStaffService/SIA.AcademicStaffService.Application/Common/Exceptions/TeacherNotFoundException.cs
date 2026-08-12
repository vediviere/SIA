using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class TeacherNotFoundException : NotFoundException
{
    public TeacherNotFoundException(Guid professorId)
        : base($"No se encontró el profesor con Id {professorId}.")
    {
    }
}