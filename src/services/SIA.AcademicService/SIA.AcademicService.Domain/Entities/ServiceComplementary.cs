using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Domain.Entities;

public sealed class ServiceComplementary
{
    private ServiceComplementary() { }

    public ServiceComplementary(
        Guid tenantId,
        Guid studyPlanId,
        bool type,
        int credit)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
        }

        if (studyPlanId == Guid.Empty)
        {
            throw new ArgumentException("El plan de estudios es obligatorio.", nameof(studyPlanId));
        }

        if (credit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(credit), "Los créditos deben ser mayores que cero.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        StudyPlanId = studyPlanId;
        Type = type;
        Credit = credit;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid StudyPlanId { get; private set; }
    public bool Type { get; private set; }
    public int Credit { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public StudyPlan? StudyPlan { get; private set; }

    public void Update(bool type, int credit)
    {
        if (credit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(credit), "Los créditos deben ser mayores que cero.");
        }

        Type = type;
        Credit = credit;
        UpdatedAtUtc = DateTime.UtcNow;
    }

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
}