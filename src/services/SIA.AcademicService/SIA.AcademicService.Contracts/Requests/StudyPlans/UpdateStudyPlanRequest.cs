using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Requests.StudyPlans;
public sealed class UpdateStudyPlanRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required DateOnly EffectiveFrom { get; set; }
}
