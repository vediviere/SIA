using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Application.DTOs.EducationalProgram
{
    public sealed class EducationalProgramFilter
    {
        public Guid TenantId { get; init; }

        public string? Code { get; init; }

        public string? Name { get; init; }

        public string? Level { get; init; }

        public bool? Status { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}
