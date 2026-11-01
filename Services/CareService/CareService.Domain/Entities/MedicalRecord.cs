using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Domain.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ConsultationId { get; set; }

        public string? Symptoms { get; set; }

        public string Diagnosis { get; set; } = string.Empty;
        public string? ClinicalNotes { get; set; }
        public string? TreatmentPlan { get; set; }

        public DateOnly? FollowUpDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
            = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }
        public Consultation Consultation { get; set; } = null!;
    }
}
