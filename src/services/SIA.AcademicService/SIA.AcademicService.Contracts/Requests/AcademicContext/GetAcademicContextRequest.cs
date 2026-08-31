using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Requests.AcademicContext;

public sealed record GetAcademicContextRequest
{
    public required Guid TenantId { get; init; }
    public required Guid EducationalProgramId { get; init; }
}