using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.AcademicStaffService.Application.Common.Exceptions;

public sealed class DuplicatePersonEmployeeNumberException : ConflictException
{
    public DuplicatePersonEmployeeNumberException(string employeeNumber)
        : base($"Ya existe una persona con el número de empleado {employeeNumber}.")
    {
    }
}