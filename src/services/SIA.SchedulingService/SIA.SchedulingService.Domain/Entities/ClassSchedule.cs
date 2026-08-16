using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace SIA.SchedulingService.Domain.Entities;

public sealed class ClassSchedule
{
    private ClassSchedule() { }

    public ClassSchedule(
        Guid tenantId,
        Guid offeringId,
        Guid classroomLabId,
        Guid academicPeriodId,
        string day,
        DateTime startTime,
        DateTime endTime)

    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));

        if (offeringId == Guid.Empty)
            throw new ArgumentException("La oferta (Offering) es obligatoria.", nameof(offeringId));

        if (classroomLabId == Guid.Empty)
            throw new ArgumentException("El aula o laboratorio es obligatorio.", nameof(classroomLabId));

        if (academicPeriodId == Guid.Empty)
            throw new ArgumentException("El periodo académico es obligatorio.", nameof(academicPeriodId));

        if (string.IsNullOrWhiteSpace(day))
            throw new ArgumentException("El día es obligatorio.", nameof(day));

        if (startTime >= endTime)
            throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.");

        Id = Guid.NewGuid();
        TenantId = tenantId;
        OfferingId = offeringId;
        ClassroomLabId = classroomLabId;
        AcademicPeriodId = academicPeriodId;
        Day = day.Trim().ToUpperInvariant();
        StartTime = startTime;
        EndTime = endTime;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid OfferingId { get; private set; }
    public Guid ClassroomLabId { get; private set; }
    public Guid AcademicPeriodId { get; private set; }
    public string Day { get; private set; } = string.Empty;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public ClassroomLab? ClassroomLab { get; private set; }
    public AcademicOffering? Offering { get; private set; }

    public void Update(
        string day,
        DateTime startTime,
        DateTime endTime)
    {
        if (string.IsNullOrWhiteSpace(day))
            throw new ArgumentException("El día es obligatorio.", nameof(day));

        if (startTime >= endTime)
            throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.");

        Day = day.Trim().ToUpperInvariant();
        StartTime = startTime;
        EndTime = endTime;
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