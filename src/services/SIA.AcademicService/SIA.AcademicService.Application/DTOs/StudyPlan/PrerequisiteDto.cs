using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.StudyPlan
{
    public sealed class PrerequisiteDto
    {
        public Guid Id { get; init; }
        public string? Code { get; init; }
        public string? Name { get; init; }
    }
}
