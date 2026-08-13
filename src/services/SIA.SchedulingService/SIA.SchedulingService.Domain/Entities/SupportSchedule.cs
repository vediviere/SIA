using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Domain.Entities;

public sealed class SupportSchedule
{
    private SupportSchedule()
    {
    }

    public SupportSchedule(
        Guid tenantId,
        Guid supportHourId,
        Guid classroomLabId,
        Guid academicPeriodId,
        string day,
        DateTime startTime,
        DateTime endTime)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));

        if (supportHourId == Guid.Empty)
            throw new ArgumentException("La hora de apoyo es obligatoria.", nameof(supportHourId));

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
        SupportHourId = supportHourId;
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
    public Guid SupportHourId { get; private set; }
    public Guid ClassroomLabId { get; private set; }
    public Guid AcademicPeriodId { get; private set; }
    public string Day { get; private set; } = string.Empty;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public ClassroomLab? ClassroomLab { get; private set; }

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