using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class ServiceComplementaryNotFoundException : Exception
{
    public ServiceComplementaryNotFoundException(Guid id)
        : base($"No se encontró la actividad complementaria con el identificador {id}.")
    {
    }
}
