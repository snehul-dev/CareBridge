using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.MedicalRecords
{
    public class MedicalRecordResponseDto
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

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
