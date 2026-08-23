using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.Common.Exceptions;

public sealed class DuplicateStudyPlanSubjectException : Exception
{
    public DuplicateStudyPlanSubjectException()
        : base("La materia ya se encuentra asignada a este plan de estudios.")
    {
    }
}