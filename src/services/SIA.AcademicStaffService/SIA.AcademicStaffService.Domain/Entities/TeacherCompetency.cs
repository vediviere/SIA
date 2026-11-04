using System;

namespace SIA.AcademicStaffService.Domain.Entities
{
    public sealed class TeacherCompetency
    {
        private TeacherCompetency()
        {
        }

        public TeacherCompetency(
            Guid tenantId,
            Guid teacherId,
            Guid competencyId,
            string level,
            string stage)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("El tenant es obligatorio.", nameof(tenantId));
            }

            if (teacherId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del docente es obligatorio.", nameof(teacherId));
            }

            if (competencyId == Guid.Empty)
            {
                throw new ArgumentException("El identificador de la competencia es obligatorio.", nameof(competencyId));
            }

            if (string.IsNullOrWhiteSpace(level))
            {
                throw new ArgumentException("El nivel es obligatorio.", nameof(level));
            }

            if (string.IsNullOrWhiteSpace(stage))
            {
                throw new ArgumentException("La etapa es obligatoria.", nameof(stage));
            }

            TeacherCompetencyId = Guid.NewGuid();
            TenantId = tenantId;
            TeacherId = teacherId;
            CompetencyId = competencyId;
            Level = level.Trim();
            Stage = stage.Trim();
            Status = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Guid TeacherCompetencyId { get; private set; }
        public Guid TenantId { get; private set; }
        public Guid TeacherId { get; private set; }
        public Guid CompetencyId { get; private set; }
        public Guid? ValidatedBy { get; private set; }
        public string Level { get; private set; } = string.Empty;
        public string Stage { get; private set; } = string.Empty;
        public DateTime? ValidateAt { get; private set; }
        public bool Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        public void Activate()
        {
            Status = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Update(string level, string stage)
        {
            if (string.IsNullOrWhiteSpace(level))
            {
                throw new ArgumentException("El nivel es obligatorio.", nameof(level));
            }

            if (string.IsNullOrWhiteSpace(stage))
            {
                throw new ArgumentException("La etapa es obligatoria.", nameof(stage));
            }

            Level = level.Trim();
            Stage = stage.Trim();
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Validate(Guid validatedBy)
        {
            if (validatedBy == Guid.Empty)
            {
                throw new ArgumentException("El usuario que valida es obligatorio.", nameof(validatedBy));
            }

            ValidatedBy = validatedBy;
            ValidateAt = DateTime.UtcNow;
            Status = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}