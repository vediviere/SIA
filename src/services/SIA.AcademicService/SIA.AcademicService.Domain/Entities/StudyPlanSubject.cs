using SIA.AcademicService.Domain.Entities;
using System;

namespace SIA.AcademicService.Domain.Entities;

public sealed class StudyPlanSubject
{
    private StudyPlanSubject()
    {
    }

    public StudyPlanSubject(
        Guid tenantId,
        Guid studyPlanId,
        Guid subjectId,
        int semester,
        int credits,
        bool isRequired)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (studyPlanId == Guid.Empty)
        {
            throw new ArgumentException("El plan de estudios es obligatorio.", nameof(studyPlanId));
        }

        if (subjectId == Guid.Empty)
        {
            throw new ArgumentException("La materia es obligatoria.", nameof(subjectId));
        }

        if (semester <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(semester), "El semestre debe ser mayor a cero.");
        }

        if (credits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(credits), "Los créditos deben ser mayores que cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        StudyPlanId = studyPlanId;
        SubjectId = subjectId;
        Semester = semester;
        Credits = credits;
        IsRequired = isRequired;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid StudyPlanId { get; private set; }
    public Guid SubjectId { get; private set; }
    public int Semester { get; private set; }
    public int Credits { get; private set; }
    public bool IsRequired { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public Subject? Subject { get; private set; }
    public StudyPlan? StudyPlan { get; private set; }

    public void SoftDelete()
    {
        Status = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        Status = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Update(int semester, int credits, bool isRequired)
    {
        if (semester <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(semester),
                "El semestre debe ser mayor que cero.");
        }

        if (credits <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(credits),
                "Los créditos deben ser mayores que cero.");
        }

        Semester = semester;
        Credits = credits;
        IsRequired = isRequired;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}