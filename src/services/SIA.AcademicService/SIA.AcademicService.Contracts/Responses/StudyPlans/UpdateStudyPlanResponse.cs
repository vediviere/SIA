using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Responses.StudyPlans;

public sealed class UpdateStudyPlanResponse
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid EducationalProgramId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required DateOnly EffectiveFrom { get; set; }
    public required bool Status { get; set; }
    public required DateTime UpdatedAtUtc { get; set; }
}