using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.DTOs.Prescriptions
{
    public class PrescriptionResponseDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public string? PatientName { get; set; }

        public Guid DoctorId { get; set; }

        public Guid ConsultationId { get; set; }

        public string? DoctorName { get; set; }

        public string? DoctorProfileImageUrl { get; set; }

        public string? DoctorSpecialization { get; set; }

        public string? Notes { get; set; }

        public string? Diagnosis { get; set; }

        public string? TreatmentPlan { get; set; }

        public string? ClinicalNotes { get; set; }

        public DateTimeOffset? PdfGeneratedAt { get; set; }

        public string? PdfDownloadUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<PrescriptionItemResponseDto> Items { get; set; } = new();
    }
}
