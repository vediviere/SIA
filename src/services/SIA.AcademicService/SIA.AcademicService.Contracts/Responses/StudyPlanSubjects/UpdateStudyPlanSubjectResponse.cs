using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Responses.StudyPlanSubjects;

public sealed class UpdateStudyPlanSubjectResponse
{
    public Guid Id { get; init; }

    public Guid TenantId { get; init; }

    public Guid StudyPlanId { get; init; }

    public Guid SubjectId { get; init; }

    public int Semester { get; init; }

    public int Credits { get; init; }

    public bool IsRequired { get; init; }

    public bool Status { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public Guid CorrelationId { get; init; }
}