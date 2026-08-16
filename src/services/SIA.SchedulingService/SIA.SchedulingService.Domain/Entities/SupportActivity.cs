using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Domain.Entities;

public sealed class SupportActivity
{
    private SupportActivity() { }

    public SupportActivity(
        Guid tenantId,
        string activity,
        string observation)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));

        if (string.IsNullOrWhiteSpace(activity))
            throw new ArgumentException("La actividad es obligatoria.", nameof(activity));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Activity = activity.Trim();
        Observation = observation.Trim() ?? string.Empty;
        Status = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; } 
    public Guid TenantId { get; private set; }
    public string Activity { get; private set; } = string.Empty;
    public string Observation { get; private set; } = string.Empty;
    public bool Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void Update(string activity, string observation)
    {
        if (string.IsNullOrWhiteSpace(activity))
            throw new ArgumentException("La actividad es obligatoria.", nameof(activity));

        Activity = activity.Trim();
        Observation = Observation.Trim() ?? string.Empty;
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